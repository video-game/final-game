using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BurrowingEnemy : Enemy
{
    [SerializeField]
    private float burrowTime = 1;

    [SerializeField]
    private float playerCheckTime;
    private float playerCheckTimer = 0;

    //How close should the enemy be to start following the player
    [SerializeField]
    private float attackDistance;

    //enemy should stop at an arm's length.
    //This variable controls that.
    [SerializeField]
    private float stopDistance;

    public GameObject bullet;
    
    private bool burrowed = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = transform.GetComponentInChildren<Animator>();
		damageTakenCanvas = GetComponentInChildren<DamageTakenCanvas>();
    }

    protected override void Hit(AbilityHitDetector projectile)
    {
        base.Hit(projectile);

        Knockback(projectile.velocity, Mathf.Abs(projectile.ability.damage));     
        Burrow();
    }

    private void Burrow()
    {
        agent.speed = 0.7f;
        burrowed = true;
        GetComponent<Collider>().enabled = false;
        animator.SetBool("IsBurrowed", true);
        StartCoroutine(BurrowCoroutine(burrowTime));
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
                Vector3 playerPosition = GetClosestPlayer();
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
                Vector3 playerPosition = GetClosestPlayer();
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

                    Vector3 zeroedPos = new Vector3(transform.position.x, 0, transform.position.z);

                    GameObject clone = Instantiate(bullet, new Vector3(transform.position.x, 0, transform.position.z), bullet.transform.rotation);
                    clone.GetComponent<Projectile>().Init((playerPosition - zeroedPos).normalized * 4, gameObject);

                    agent.isStopped = (agent.remainingDistance < stopDistance);
                }
            }
        }
	} 
}
