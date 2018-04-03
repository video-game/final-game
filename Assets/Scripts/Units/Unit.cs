using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Unit : MonoBehaviour
{
    public delegate void StatChangeDelegate(int current, int max);
    public StatChangeDelegate OnHealthChange;

    [SerializeField]
    protected int maxHealth;
    public int MaxHealth { get { return maxHealth; } }

    [SerializeField]
    protected int currentHealth;
    public int CurrentHealth { get { return currentHealth; } }

    protected bool alive = true;
    public bool Alive { get { return alive; } }

    protected bool invincible;

    [SerializeField]
    protected GameObject tombstone;
    
    protected NavMeshAgent agent;
    protected DamageTakenCanvas damageTakenCanvas;
    protected Animator animator;

    protected GameObject lastAttacker;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = transform.GetComponentInChildren<Animator>();
        damageTakenCanvas = GetComponentInChildren<DamageTakenCanvas>();
    }

    public virtual void ChangeHealth(int value)
    {
        damageTakenCanvas.InitializeDamageText(value);

        currentHealth = Mathf.Clamp(currentHealth + value, 0, maxHealth);

        if (OnHealthChange != null)
            OnHealthChange(currentHealth, maxHealth);

        if (currentHealth == 0)
            Die();
    }

    protected virtual void Hit(Projectile projectile)
    {
        if (!invincible)
        {
            ChangeHealth(projectile.damage);
            lastAttacker = projectile.Shooter;
        }
    }

    protected virtual void Die()
    {
        alive = false;

        if (tombstone != null)
        {
            var clone = Instantiate(tombstone);
            clone.transform.position = transform.position;
        }

        this.gameObject.SetActive(false);
    }

    protected void Knockback(Vector3 direction, float power)
    {
        if (alive)
        {
            agent.ResetPath();
            direction = direction.normalized;
            agent.velocity = new Vector3(direction.x, 0, direction.z) * power;
        }
    }
}
