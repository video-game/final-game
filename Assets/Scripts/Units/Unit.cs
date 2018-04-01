using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    public delegate void StatChangeDelegate(int current, int max);
    public StatChangeDelegate OnHealthChange;

    public delegate void OnDeathDelegate();
    public OnDeathDelegate OnDeath;

    [SerializeField]
    protected int maxHealth;

    [SerializeField]
    protected int currentHealth;

    protected bool alive = true;
    public bool Alive { get { return alive; } }

    public virtual void ChangeHealth(int value)
    {
        currentHealth = Mathf.Clamp(currentHealth + value, 0, maxHealth);

        if (OnHealthChange != null)
        {
            OnHealthChange(currentHealth, maxHealth);
        }

        if (currentHealth == 0)
        {
            Dead();
        }
    }

    protected virtual void Dead()
    {
        alive = false;

        if (OnDeath != null)
            OnDeath();

        var tombstone = Instantiate(GameManager.Instance.Tombstone);
        tombstone.transform.position = transform.position;

        this.gameObject.SetActive(false);
    }
}
