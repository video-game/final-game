using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemy : Enemy
{
    [SerializeField]
    private float playerCheckTime;
    private float playerCheckTimer = 0;

    private NavMeshAgent agent;

    //How close should the enemy be to start following the player
    [SerializeField]
    private float attackDistance;

    //enemy should stop at an arm's length.
    //This variable controls that.
    [SerializeField]
    private float stopDistance;

    [SerializeField]
    private float shootSpeed;
    private float shootTimer;

    public GameObject bullet;

    private bool attacking;

    private Animator animator;
    private DamageTakenCanvas damageTakenCanvas;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = transform.GetComponentInChildren<Animator>();
		damageTakenCanvas = GetComponentInChildren<DamageTakenCanvas>();
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "PlayerProjectile")
        {
            Knockback(collision.gameObject.GetComponent<DemoProjectile>().velocity, 10);
            ChangeHealth(-10);
            damageTakenCanvas.InitializeDamageText(10.ToString());
            attacking = true;
        }
    }

    private void Knockback(Vector3 direction, float power)
    {
        //normalize the vector, just to be sure
        direction = direction.normalized;
        agent.velocity = new Vector3(direction.x, 0, direction.z) * power;
        agent.SetDestination((new Vector3(direction.x, 0, direction.z) + transform.position));
    }

    void Update ()
    {
        animator.SetBool("IsRunning", agent.velocity != Vector3.zero);

        //NOTE: this code assumes that there is 1 player only. Will need fixing if we do 2 player.
        Vector3 playerPosition = GetClosestPlayer();

        if (playerPosition != null && attacking)
        {
            shootTimer += Time.deltaTime;
            if(shootTimer > shootSpeed)
            {
                shootTimer = 0;

                Vector3 zeroedPos = new Vector3(transform.position.x, 0, transform.position.z);

                GameObject clone = Instantiate(bullet, new Vector3(transform.position.x, 0, transform.position.z), bullet.transform.rotation);
                clone.GetComponent<DemoProjectile>().init((playerPosition - zeroedPos).normalized * 4);

                //TODO make this work
                //for some reason raycasting on the mesh doesn't work so we just have to make it shoot at blockades I guess

                //The NavMeshAgent tends to set the y value to something not quite 0,
                //and the environment mesh is only exactly at y=0.
                //So we must fix this by raycasting a y zeroed vector.
                /*
                RaycastHit hit;
                Debug.DrawRay(zeroedPos, (playerPosition - zeroedPos), Color.red, playerCheckTime);
                
                if (Physics.Raycast(zeroedPos, (playerPosition - zeroedPos), out hit))
                {
                    Debug.Log("hit something! " + hit.transform);
                }
                */
            }
        }

        playerCheckTimer += Time.deltaTime;
        if (playerPosition != null && playerCheckTimer > playerCheckTime)
        {
            playerCheckTimer = 0;
            NavMeshPath path = new UnityEngine.AI.NavMeshPath();

            //calculate the distance of path to player
            agent.CalculatePath(playerPosition, path);
            float distance = Utilities.PathDistance(path);
            if (attackDistance > distance)
                attacking = true;

            //If enemy is aggro, move towards him and try to shoot him (if he isn't behind a wall).
            if (attacking)
            {
                agent.SetDestination(playerPosition);

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
