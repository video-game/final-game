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

    private Animator animator;
    private Transform hand;

    private bool dashOnCooldown;
    private bool isWalking;

    private NavMeshAgent agent;

    private Vector3 aimDirection;

    private Color bodyColor;

    Transform projectileSpawn;

    private bool dashing; //for stopping other movement during dashing
    private Vector3 dashVelocity;

    [SerializeField]
    private float damageRecoveryTime;
    private bool invincible;
    [HideInInspector]
    public bool KOd;

    [SerializeField]
    private int maxKoHealth;
    private int currentKoHealth;

    private void Awake()
    {

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

        maxHealth = 100;
        currentHealth = maxHealth;

        projectileScript = demoProjectile.GetComponent<PlayerProjectile>();
        dashing = false;
        invincible = false;
        KOd = false;
        fireOnDelay = false;
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
            cloneScript.init(9 * aimDirection);

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
            cloneScript.init(9 * aimDirection);

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
        if(collision.gameObject.tag == "EnemyProjectile")
        {
            var stats = collision.gameObject.GetComponent<DemoProjectile>();
            Damaged(stats.damage, collision.gameObject.GetComponent<DemoProjectile>().velocity);
        }
    }

    public void Damaged(int damage, Vector3 damageDirection)
    {
        int damageAbs = Mathf.Abs(damage);
        if (!invincible)
        {
            ChangeHealth(damage);

            StartCoroutine(DamagedCoroutine(damageRecoveryTime));
        }
        //figured it would be clever to have a constant to 
        //translate amount of damage taken to screen shake magnitude
        float damageToShakeRatio = 0.0025f;
        Camera.main.GetComponent<CameraEffects>().ShakeCamera(0.05f, damageAbs * damageToShakeRatio);

        Knockback(damageDirection, damageAbs);

    }

    public override void ChangeHealth(int value)
    {
        if(!KOd)
        {
            base.ChangeHealth(value);
        }
        else
        {
            currentKoHealth = (currentKoHealth + value) < 0 ? 0 : (currentKoHealth + value) > maxKoHealth ? maxKoHealth : (currentKoHealth + value);
            if (OnHealthChange != null)
            {
                OnHealthChange(currentKoHealth, maxKoHealth);
            }

            if(currentKoHealth == 0)
            {
                base.Dead();
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

    private void Knockback(Vector3 direction, float power)
    {
        //normalize the vector, just to be sure
        direction.Normalize();
        agent.ResetPath();
        agent.velocity = new Vector3(direction.x, 0, direction.z) * power;
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
                if(player.KOd)
                {
                    player.Revive();
                }
            }
        }
    }

    //base onHealthChanged calls this function when it runs out of health
    //if there is more than 1 player, it's a bit of a misnomer. 
    //Player gets KOed if there are 2 players, otherwise he just dies.
    protected override void Dead()
    {
        if(GameManager.Instance.player.Count > 1)
        {
            animator.SetBool("KOd", true);
            KOd = true;

            currentKoHealth = maxKoHealth;
            OnPlayerKO();
            OnHealthChange(maxKoHealth, maxKoHealth);
        }
        else
        {
            base.Dead();
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