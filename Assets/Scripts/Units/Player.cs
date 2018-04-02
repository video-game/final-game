using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Player : Unit
{
    public delegate void PlayerDelegate();
    public PlayerDelegate OnPlayerDeath;
    public PlayerDelegate OnPlayerKO;
    public PlayerDelegate OnPlayerRevive;

    [SerializeField]
    private float speed;

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

    private int nextLevel = 100;

    public void GrantExperience(int amount)
    {
        experience += amount;

        if (experience >= nextLevel)
            levelUp();
    }

    private void levelUp()
    {
        level++;
        experience -= nextLevel;
        if (experience < 0)
            experience = 0;
        nextLevel = (int)(nextLevel * (1 + (level / 10f)));
        Debug.Log("Level up! You are now level " + level + "! XP until next level: " + nextLevel);

        // Temporary
        maxHealth += 10;
        ChangeHealth(maxHealth);
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
            cloneScript.init(projectileScript.speed * aimDirection, gameObject);

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
            cloneScript.init(9 * aimDirection, gameObject);

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
            Hit(collision.gameObject.GetComponent<DemoProjectile>());
    }

    protected override void Hit(DemoProjectile projectile)
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

    public override void ChangeHealth(int value)
    {
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
        //check if any KOed players are in a 1 unit radius
        foreach(var player in GameManager.Instance.player)
        {
            if(player != this && Vector3.Distance(transform.position, player.transform.position) < 1)
            {
                if(player.KOd && player.isActiveAndEnabled)
                {
                    if(GameManager.Instance.resourceHud.UpdateRevives(-1))
                    {
                        player.Revive();
                        return;
                    }
                }
            }
        }

        //This is a ludicrously bad idea as it forces a parent gameObject for villagers
        //But time constraints force bad coding habits
        foreach(Transform villager in GameObject.Find("Villagers").transform)
        {
            Villager vScript = villager.GetComponent<Villager>();
            if(vScript.InRange(transform.position))
            {
                vScript.Interact();
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