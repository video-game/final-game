using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BurrowingEnemy : MonoBehaviour {

    [SerializeField]
    private float playerCheckTime;
    private float playerCheckTimer = 0;

    NavMeshAgent agent;

    //How close should the enemy be to start following the player
    [SerializeField]
    private float attackDistance;
    //How far does the player have to be for the agent to give up
    [SerializeField]
    private float stopAttackDistance;
    //enemy should stop at an arm's length.
    //This variable controls that.
    [SerializeField]
    private float stopDistance;

    private bool inAttackDistance = false;

    public GameObject bullet;

    public float health;

    Animator animator;
	private DamageTakenCanvas damageTakenCanvas;

    private bool burrowed = false;
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = transform.GetComponentInChildren<Animator>();
		damageTakenCanvas = GetComponentInChildren<DamageTakenCanvas>();
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "PlayerProjectile")
        {
            Knockback(collision.gameObject.GetComponent<DemoProjectile>().velocity, 10);
            health -= 10;
            damageTakenCanvas.InitializeDamageText(10.ToString());

            Burrow();

            if(health < 1)
            {
                var tombstone = Instantiate(GameManager.Instance.Tombstone);
                tombstone.transform.position = transform.position;

                damageTakenCanvas.Orphan();
                Destroy(this.gameObject);
            }
        }
    }

    private void Knockback(Vector3 direction, float power)
    {
        //normalize the vector, just to be sure
        direction = direction.normalized;
        agent.velocity = new Vector3(direction.x, 0, direction.z) * power;
        agent.SetDestination((new Vector3(direction.x, 0, direction.z) + transform.position));
    }

    private void Burrow()
    {
        agent.speed = 0.7f;
        burrowed = true;
        GetComponent<Collider>().enabled = false;
        animator.SetBool("IsBurrowed", true);
        StartCoroutine(BurrowCoroutine(2));
    }

    IEnumerator BurrowCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        UnBurrow();
    }

    private void UnBurrow()
    {
        agent.speed = 3;
        burrowed = false;
        GetComponent<Collider>().enabled = true;
        animator.SetBool("IsBurrowed", false);
    }

    void Update ()
    {
        playerCheckTimer += Time.deltaTime;

        //Burrowing enemy has 2 behavioral patterns:
        //1. Attacking the player above ground.
        //2. If he gets attacked he flees from the player and burrows underground.
        if (burrowed)
        {
            //NOTE: this code assumes that there is 1 player only. Will need fixing if we do 2 player.
            if (playerCheckTimer > playerCheckTime)
            {
                Vector3 playerPosition = GameManager.Instance.player[0].transform.position;
                Vector3 fleeDirection = (transform.position - playerPosition).normalized;

                agent.SetDestination(transform.position + fleeDirection);
            }

        }
        else
        {
            animator.SetBool("IsRunning", agent.velocity != Vector3.zero);

            
            if (playerCheckTimer > playerCheckTime)
            {
                playerCheckTimer = 0;
                //NOTE: this code assumes that there is 1 player only. Will need fixing if we do 2 player.
                Vector3 playerPosition = GameManager.Instance.player[0].transform.position;
                NavMeshPath path = new UnityEngine.AI.NavMeshPath();

                //calculate the distance of path to player
                agent.CalculatePath(playerPosition, path);
                float distance = Utilities.PathDistance(path);
                if (attackDistance > distance)
                {
                    inAttackDistance = true;
                }
                else if (stopAttackDistance < distance)
                {
                    inAttackDistance = false;
                }

                //If enemy is aggro, move towards him and try to shoot him (if he isn't behind a wall).
                if (inAttackDistance)
                {
                    agent.SetDestination(playerPosition);

                    Vector3 zeroedPos = new Vector3(transform.position.x, 0, transform.position.z);

                    GameObject clone = Instantiate(bullet, new Vector3(transform.position.x, 0, transform.position.z), bullet.transform.rotation);
                    clone.GetComponent<DemoProjectile>().init((playerPosition - zeroedPos).normalized * 4);


                    if (agent.remainingDistance < stopDistance)
                    {
                        agent.isStopped = true;
                    }
                    else
                    {
                        agent.isStopped = false;
                    }
                }
            }
        }
	}

    
}
