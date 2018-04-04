using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Unit : MonoBehaviour
{
    public delegate void StatChangeDelegate(int current, int max);
    public StatChangeDelegate OnHealthChange;

    public int MaxHealth;

    [SerializeField]
    protected int currentHealth;
    public int CurrentHealth { get { return currentHealth; } }

    protected bool alive = true;
    public bool Alive { get { return alive; } }

    protected bool invincible;

    protected Vector3 aimDirection;
    public Vector3 AimDirection { get { return aimDirection; } }

    protected Transform projectileSpawn;
    public Transform ProjectileSpawn { get { return projectileSpawn; } }

    [SerializeField]
    protected GameObject tombstone;
    
    protected NavMeshAgent agent;
    protected DamageTakenCanvas damageTakenCanvas;
    protected Animator animator;

    protected GameObject lastAttacker;

    public bool dashing;
    public Vector3 dashingVelocity;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = transform.GetComponentInChildren<Animator>();
        damageTakenCanvas = GetComponentInChildren<DamageTakenCanvas>();

        projectileSpawn = transform;
    }

    public virtual void ChangeHealth(int value)
    {
        damageTakenCanvas.InitializeDamageText(value);

        currentHealth = Mathf.Clamp(currentHealth + value, 0, MaxHealth);

        if (OnHealthChange != null)
            OnHealthChange(currentHealth, MaxHealth);

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

    protected bool HasLineOfSight(Vector3 target)
    {
        NavMeshHit dummy;
        bool blocked = NavMesh.Raycast(transform.position, target, out dummy, NavMesh.GetAreaFromName("Movable"));
        Debug.DrawLine(transform.position, target, blocked ? Color.red : Color.green);
        return !blocked;
    }
}
