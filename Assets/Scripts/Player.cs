using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
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
    private Rigidbody body;
    private Transform hand;

    private bool dashOnCooldown;
    private bool isWalking;

    private NavMeshAgent agent;

    private Vector3 aimDirection;

    private Color bodyColor;

    Transform projectileSpawn;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        body = GetComponentInChildren<Rigidbody>();
        hand = transform.Find("Hand");
        projectileSpawn = hand.GetChild(0).transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        bodyColor = transform.Find("Model").GetComponent<SpriteRenderer>().color;
    }
    //player move function
    public void Move(float horizontal, float vertical)
    {
        isWalking = (horizontal != 0 || vertical != 0);
        animator.SetBool("IsWalking", isWalking);

        //set destination right infront of player. (player navMeshAgent has high acceleration)
        //good way of thinking about it, is like tying a hotdog on a stick to a dog.
        Vector3 movePos = new Vector3(transform.position.x + horizontal, 0, transform.position.z + vertical);
        agent.SetDestination(movePos);
    }

    //player dash function
    public void Dash()
    {
        if (!dashOnCooldown)
        {
            StartCoroutine(DashCoroutine());
            if (dashCooldown > 0f)
                StartCoroutine(DashCooldown());
        }
    }

    private IEnumerator DashCoroutine()
    {
        Vector3 currentDirection = agent.velocity.normalized;
        agent.velocity = new Vector3(currentDirection.x, 0, currentDirection.z) * dashSpeed;
        yield return new WaitForSeconds(dashDuration);
        agent.velocity = Vector2.zero;
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
        clone.velocity = 9 * aimDirection;
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
            Damaged();
            Knockback(collision.gameObject.GetComponent<DemoProjectile>().velocity, 10);
        }
    }

    public void Damaged()
    {
        StartCoroutine(DamagedCoroutine(0.5f));
    }


    //coroutine for when player is damaged, colors the player red for some time
    IEnumerator DamagedCoroutine(float duration)
    {
        float colorIntensity = 10;

        var modelRenderer = transform.Find("Model").GetComponent<SpriteRenderer>();
        var handRenderer = transform.Find("Hand").GetComponentInChildren<SpriteRenderer>();
        
        modelRenderer.color = new Color(bodyColor.r, bodyColor.g - colorIntensity, bodyColor.b - colorIntensity);
        handRenderer.color = new Color(bodyColor.r, bodyColor.g - colorIntensity, bodyColor.b - colorIntensity);

        yield return new WaitForSeconds(duration);
        
        modelRenderer.color = bodyColor;
        handRenderer.color = bodyColor;
    }

    private void Knockback(Vector3 direction, float power)
    {
        //normalize the vector, just to be sure
        direction.Normalize();
        agent.velocity = new Vector3(direction.x, 0, direction.z) * power;
        agent.SetDestination(new Vector3(direction.x, 0, direction.z) + transform.position);
    }
}