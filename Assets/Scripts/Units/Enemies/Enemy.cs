using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : Unit
{
    // Delegate that is called just before object is destroyed
    protected delegate void OnDeathDelegate();
    protected OnDeathDelegate OnDeath;

    [SerializeField]
    protected List<Drop> Drops;
    protected bool attacking;
    [SerializeField]
    protected int experienceAward;

    protected override void Hit(DemoProjectile projectile)
    {
        base.Hit(projectile);

        Knockback(projectile.velocity, Mathf.Abs(projectile.damage));
    }
    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "PlayerProjectile")
        {
            Hit(collision.gameObject.GetComponent<DemoProjectile>());
            attacking = true;
        }
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

    protected override void Die()
    {
        base.Die();

        // If the last attacker was a player
        if (lastAttacker != null && lastAttacker.GetComponent<Player>() != null)
            lastAttacker.GetComponent<Player>().GrantExperience(experienceAward);

        foreach (var drop in Drops)
        {
            var roll = Random.value;

            if (roll <= drop.Chance)
                Instantiate(drop.Pickup, transform.position, drop.Pickup.transform.rotation);
        }

        if (OnDeath != null)
            OnDeath();

		damageTakenCanvas.Orphan();
        Destroy(gameObject);
    }
}
