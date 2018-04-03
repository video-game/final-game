using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemy : Enemy
{
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

    [SerializeField]
    private float shootSpeed;
    private float shootTimer;

    public GameObject bullet;

    protected override void Hit(Projectile projectile)
    {
        base.Hit(projectile);

        Knockback(projectile.velocity, Mathf.Abs(projectile.damage));
    }

    void Update()
    {
        animator.SetBool("IsWalking", agent.velocity != Vector3.zero);
        animator.SetInteger("Direction", (int)Utilities.VectorToDirection(agent.velocity.x, agent.velocity.z));

        Vector3 playerPosition = GetClosestPlayer();

        if (playerPosition != Vector3.zero && attacking)
        { 
    
            shootTimer += Time.deltaTime;
            if(shootTimer > shootSpeed && HasLineOfSight(playerPosition))
            {
                shootTimer = 0;

                Vector3 zeroedPos = new Vector3(transform.position.x, 0, transform.position.z);

                GameObject clone = Instantiate(bullet, new Vector3(transform.position.x, 0, transform.position.z), bullet.transform.rotation);
                clone.GetComponent<Projectile>().Init((playerPosition - zeroedPos).normalized * 4, gameObject);
            }
        }

        playerCheckTimer += Time.deltaTime;
        if (playerPosition != Vector3.zero && playerCheckTimer > playerCheckTime)
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
