using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Player : Unit
{
    public delegate void PlayerDelegate();
    public PlayerDelegate OnPlayerDeath;

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

        dashing = false;
        invincible = false;
    }

    //player move function
    public void Move(float horizontal, float vertical)
    {
        if (dashing)
        {
            agent.velocity = dashVelocity;
            Debug.Log("velocity: " + agent.velocity + " dash velocity: " + dashVelocity);
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
        agent.ResetPath();
        yield return new WaitForSeconds(dashDuration);
        dashing = false;
    }

    private IEnumerator DashCooldown()
    {
        dashOnCooldown = true;
        yield return new WaitForSeconds(dashCooldown);
        dashOnCooldown = false;
    }

    public void Shoot()
    {
        //spawn projectile, set it's trajectory
        Rigidbody clone = (Rigidbody)Instantiate(demoProjectile, new Vector3(projectileSpawn.position.x, 0, projectileSpawn.position.z), demoProjectile.transform.rotation);
        clone.GetComponent<DemoProjectile>().init(9 * aimDirection);
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
            Damaged(-10, collision.gameObject.GetComponent<DemoProjectile>().velocity);
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

    protected override void Dead()
    {
        base.Dead();

        OnPlayerDeath();
    }
}