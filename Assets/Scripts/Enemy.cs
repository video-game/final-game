using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Unit
{
    public List<Drop> Drops;

    protected NavMeshAgent agent;

    protected bool attacking;
    
    protected DamageTakenCanvas damageTakenCanvas;

    protected Animator animator;

    public virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = transform.GetComponentInChildren<Animator>();
        damageTakenCanvas = GetComponentInChildren<DamageTakenCanvas>();
    }

    public Enemy()
    {
        OnDeath += RollDrops;
    }

    protected Vector3 GetClosestPlayer()
    {
        Vector3 min = new Vector3(0, 0, 0);
        float distance = Mathf.Infinity;

        for (int i = 0; i < GameManager.Instance.player.Count; i++)
        {
            if (!GameManager.Instance.player[i].Alive)
            {
                continue;
            }

            float temp = Vector3.Distance(GameManager.Instance.player[i].transform.position, transform.position);
            if (temp < distance)
            {
                min = GameManager.Instance.player[i].transform.position;
                distance = temp;
            }
        }

        return min;
    }

    private void RollDrops()
    {
        foreach (var drop in Drops)
        {
            var roll = Random.value;

            if (roll <= drop.Chance)
                Instantiate(drop.Pickup, transform.position, drop.Pickup.transform.rotation);
        }
    }

    public virtual void OnCollisionEnter(Collision collision)
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
}
