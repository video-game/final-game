using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : Unit, INTERACTABLE
{
    public delegate void PlayerDelegate();
    public PlayerDelegate OnPlayerDeath;
    public PlayerDelegate OnPlayerKO;
    public PlayerDelegate OnPlayerRevive;
    public PlayerDelegate OnPlayerLvlUp;
    public StatChangeDelegate OnExperienceGained;

    [SerializeField]
    private float dashSpeed;

    [SerializeField]
    private float dashDuration;

    [SerializeField]
    private float dashCooldown;

    [SerializeField]
    private Rigidbody demoProjectile; // prefab
    private PlayerProjectile projectileScript; //its script
    private bool fireOnDelay;

    private Transform hand;

    private bool dashOnCooldown;
    private bool isWalking;

    private Vector3 aimDirection;

    private Color bodyColor;

    Transform projectileSpawn;

    private bool dashing; //for stopping other movement during dashing
    private Vector3 dashVelocity;

    [SerializeField]
    private float damageRecoveryTime;
    [HideInInspector]
    public bool KOd;

    [SerializeField]
    private int maxKoHealth;
    private int currentKoHealth;

    private int level = 1;
    public int Level { get { return level; } }

    private int experience = 0;
    public int Experience { get { return experience; } }

    public int nextLevel = 100;


    public SharedItem item;


    private List<INTERACTABLE> InteractList = new List<INTERACTABLE>();

    private float initialSpeed;

    protected override void Awake()
    {
        base.Awake();
        initialSpeed = agent.speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        INTERACTABLE temp = other.GetComponent<INTERACTABLE>();
        if (temp != null)
        {
            InteractList.Add(temp);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        INTERACTABLE temp = other.GetComponent<INTERACTABLE>();
        if (temp != null)
        {
            InteractList.Remove(temp);
        }
    }

    public void GrantExperience(int amount)
    {
        experience += amount;
        
        if (experience >= nextLevel)
            LevelUp();

        OnExperienceGained(experience, nextLevel);
    }

    public void Teleport(Vector3 destination)
    {
        agent.Warp(destination);
    }

    private void LevelUp()
    {
        AudioManager.Instance.PlayAudioClip("LevelUp");
        damageTakenCanvas.InitializeLevelUpText();

        level++;
        experience -= nextLevel;
        if (experience < 0)
            experience = 0;
        
        LevelUpPackage package = GetComponent<LevelingData>().GetLevelStats(level, nextLevel);
        Debug.Log("old next level: " + nextLevel);
        nextLevel = package.nextLevel;
        maxHealth += package.healthUp;
        if(package.projectile != null)
        {
            demoProjectile = package.projectile;
        }

        ChangeHealth(maxHealth);
        OnPlayerLvlUp();
        OnExperienceGained(experience, nextLevel);
        Debug.Log("Level up! You are now level " + level + "! XP until next level: " + nextLevel);
    }

    public void Init(GameObject model)
    {
        GameObject m = Instantiate(model, transform);
        m.name = "Model";
        animator = GetComponentInChildren<Animator>();
        hand = transform.Find("Hand");
        projectileSpawn = hand.GetChild(0).transform;
        agent = GetComponent<NavMeshAgent>();

        bodyColor = m.GetComponent<SpriteRenderer>().color;

        projectileScript = demoProjectile.GetComponent<PlayerProjectile>();

        item = GameManager.Instance.sharedItems;
    }

    public void Slow(float percentage, float duration)
    {
        if (agent.speed == initialSpeed)
            StartCoroutine(slow(percentage, duration));
    }

    private IEnumerator slow(float percentage, float duration)
    {
        agent.speed = initialSpeed * (1 - percentage);
        yield return new WaitForSeconds(duration);
        agent.speed = initialSpeed;
    }

    //player move function
    public void Move(float horizontal, float vertical)
    {
        if(!KOd)
        {
            if (dashing)
            {
                agent.velocity = dashVelocity;
            }
            else
            {
                isWalking = (horizontal != 0 || vertical != 0);
                animator.SetBool("IsWalking", isWalking);

                //set destination right infront of player. (player navMeshAgent has high acceleration)
                //good way of thinking about it, is like tying a hotdog on a stick to a dog.
                Vector3 movePos = new Vector3(transform.position.x + horizontal, 0, transform.position.z + vertical);
                agent.SetDestination(movePos);
            }
        }
    }

    //player dash function
    public void Dash()
    {
        if (!dashOnCooldown)
        {
            dashVelocity = agent.velocity.normalized * dashSpeed;
            StartCoroutine(DashCoroutine());
            if (dashCooldown > 0f)
                StartCoroutine(DashCooldown());
        }
    }

    private IEnumerator DashCoroutine()
    {
        dashing = true;
        TrailRenderer tRenderer = transform.Find("Model").GetComponent<TrailRenderer>();
        tRenderer.time = 0.3f;
        tRenderer.enabled = true;
        agent.ResetPath();
        yield return new WaitForSeconds(dashDuration);
        tRenderer.time = 0.1f;
        dashing = false;

        yield return new WaitForSeconds(0.1f);
        tRenderer.enabled = false;
    }

    private IEnumerator DashCooldown()
    {
        dashOnCooldown = true;
        yield return new WaitForSeconds(dashCooldown);
        dashOnCooldown = false;
    }

    //If a projectile should only fire once per trigger press
    public void Shoot()
    {
        if(!projectileScript.continuousFire && !fireOnDelay)
        {
            //spawn projectile, set it's trajectory
            Rigidbody clone = (Rigidbody)Instantiate(demoProjectile, new Vector3(projectileSpawn.position.x, 0, projectileSpawn.position.z), demoProjectile.transform.rotation);
            var cloneScript = clone.GetComponent<PlayerProjectile>();
            cloneScript.Init(projectileScript.speed * aimDirection, gameObject);

            StartCoroutine(ShootDelay(cloneScript.shootDelay));
        }
    }

    //If a projectile should fire continuously with delay
    public void ShootContinuous()
    {
        if (projectileScript.continuousFire && !fireOnDelay)
        {
            //spawn projectile, set it's trajectory
            Rigidbody clone = (Rigidbody)Instantiate(demoProjectile, new Vector3(projectileSpawn.position.x, 0, projectileSpawn.position.z), demoProjectile.transform.rotation);
            var cloneScript = clone.GetComponent<PlayerProjectile>();
            cloneScript.Init(9 * aimDirection, gameObject);

            StartCoroutine(ShootDelay(cloneScript.shootDelay));
        }
    }

    private IEnumerator ShootDelay(float delay)
    {
        fireOnDelay = true;
        yield return new WaitForSeconds(delay);
        fireOnDelay = false;
    }

    public void AimInDirection(Vector3 direction)
    {
        //set aim direction (used for shooting)
        aimDirection = direction;

        //calculate rotation Quaternion
        Quaternion rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, Quaternion.LookRotation(aimDirection, Vector3.up).eulerAngles.y,
            transform.rotation.eulerAngles.z);

        //set rotation
        hand.rotation = rotation;

        //set animator Directon
        animator.SetInteger("Direction", (int)Utilities.VectorToDirection(direction.x, direction.z));
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "EnemyProjectile")
            Hit(collision.gameObject.GetComponent<Projectile>());
    }

    protected override void Hit(Projectile projectile)
    {
        base.Hit(projectile);

        if (!invincible)
        {
            StartCoroutine(DamagedCoroutine(damageRecoveryTime));

            //figured it would be clever to have a constant to 
            //translate amount of damage taken to screen shake magnitude
            float damageToShakeRatio = 0.0025f;
            Camera.main.GetComponent<CameraEffects>().ShakeCamera(0.05f, projectile.damage * damageToShakeRatio);

            Knockback(projectile.velocity, Mathf.Abs(projectile.damage));
        }
    }

    // Damages player and ignores invincibility
    public void TakeTrueDamage(GameObject attacker, int damage)
    {
        ChangeHealth(damage);
        lastAttacker = attacker;
    }

    public override void ChangeHealth(int value)
    {
        if(value > 0)
        {
            AudioManager.Instance.PlayAudioClip("Heal", 20f);
        }
        else
        {
            AudioManager.Instance.PlayAudioClip("PlayerDamage", 20f);
        }

        if (!KOd)
            base.ChangeHealth(value);
        else
        {
            currentKoHealth = Mathf.Clamp(currentKoHealth + value, 0, maxKoHealth);

            if (OnHealthChange != null)
                OnHealthChange(currentKoHealth, maxKoHealth);

            if (currentKoHealth == 0)
            {
                base.Die();
                OnPlayerDeath();
            }
        }
    }

    //coroutine for when player is damaged, colors the player red for some time
    IEnumerator DamagedCoroutine(float duration)
    {
        float colorIntensity = 10;

        var modelRenderer = transform.Find("Model").GetComponent<SpriteRenderer>();
        var handRenderer = transform.Find("Hand").GetComponentInChildren<SpriteRenderer>();
        
        modelRenderer.color = new Color(bodyColor.r, bodyColor.g - colorIntensity, bodyColor.b - colorIntensity);
        handRenderer.color = new Color(bodyColor.r, bodyColor.g - colorIntensity, bodyColor.b - colorIntensity);

        invincible = true;
        yield return new WaitForSeconds(duration);
        invincible = false;

        modelRenderer.color = bodyColor;
        handRenderer.color = bodyColor;
    }

    //When the player's action button is pressed
    //the interact function is called.
    //Used to speak to villagers/revive teammate.
    public void Interact()
    {
        for(int i = 0; i < InteractList.Count; i++)
        {
            InteractList[i].Interaction();
        }
    
    }

    //when another player interacts with you.
    public void Interaction()
    {
        if (KOd)
        {
            if (item.ChangeRevives(-1))
            {
                Revive();
                return;
            }
        }
    }

    //base onHealthChanged calls this function when it runs out of health
    //if there is more than 1 player, it's a bit of a misnomer. 
    //Player gets KOed if there are 2 players, otherwise he just dies.
    protected override void Die()
    {
        if (GameManager.Instance.player.Count > 1)
        {
            AudioManager.Instance.PlayAudioClip("KOd");

            animator.SetBool("KOd", true);
            KOd = true;

            currentKoHealth = maxKoHealth;
            OnPlayerKO();
            OnHealthChange(maxKoHealth, maxKoHealth);
        }
        else
        {
            base.Die();
            OnPlayerDeath();
        }
    }

    private void Revive()
    {
        animator.SetBool("KOd", false);
        KOd = false;
        
        OnPlayerRevive();
        ChangeHealth(maxHealth / 3);
    }
}