using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ape : Enemy {


    [SerializeField]
    private float playerCheckTime;
    private float playerCheckTimer = 0;

    //How close should the enemy be to start following the player
    [SerializeField]
    private float attackDistance;

    [SerializeField]
    public float attackRecharge;
    private float attackRechargeTimer;

    public GameObject bullet;

    private bool recharging = false;

    private void Start()
    {
        attackRechargeTimer = attackRecharge;
    }

    void Update()
    {
        if (recharging)
            return;

        animator.SetBool("IsWalking", agent.velocity != Vector3.zero);
        animator.SetInteger("Direction", (int)Utilities.VectorToDirection(agent.velocity.x, agent.velocity.z));

        //NOTE: this code assumes that there is 1 player only. Will need fixing if we do 2 player.
        Vector3 playerPosition = GetClosestPlayer();

        if (playerPosition != null && attacking)
        {
            attackRechargeTimer += Time.deltaTime;
            if (attackRechargeTimer > attackRecharge && Vector3.Distance(playerPosition, transform.position) < 1)
            {
                attackRechargeTimer = 0;
                StartCoroutine(MoveStop(playerPosition));
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
            }
        }
    }

    protected override void Hit(Projectile projectile)
    {
        base.Hit(projectile);

        // Knockback power 20% of damage
        Knockback(projectile.velocity, Mathf.Abs(projectile.damage / 5));
    }

    private IEnumerator MoveStop(Vector3 playerPos)
    {
        recharging = true;
        agent.ResetPath();
        agent.velocity = Vector3.zero;

        animator.SetBool("Attacking", true);
        yield return new WaitForSeconds(0.2f);
        animator.SetBool("Attacking", false);

        Vector3 zeroedPos = new Vector3(transform.position.x, 0, transform.position.z);
        GameObject clone = Instantiate(bullet, new Vector3(transform.position.x, 0, transform.position.z), bullet.transform.rotation);
        clone.GetComponent<Projectile>().Init((playerPos - zeroedPos).normalized * 4, gameObject);

        recharging = false;
    }
}
