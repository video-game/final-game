using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    public delegate void StatChangeDelegate(int current, int max);
    public StatChangeDelegate OnHealthChange;

    protected int maxHealth;
    protected int currentHealth;

    protected bool alive = true;
    public bool Alive { get { return alive; } }

    public void ChangeHealth(int value)
    {
        currentHealth = (currentHealth + value) < 0 ? 0 : (currentHealth + value) > maxHealth ? maxHealth : (currentHealth + value);

        Debug.Log(currentHealth);

        if (OnHealthChange != null)
        {
            OnHealthChange(currentHealth, maxHealth);
        }

        if(currentHealth == 0)
        {
            Dead();
        }
    }

    protected virtual void Dead()
    {
        alive = false;
        var tombstone = Instantiate(GameManager.Instance.Tombstone);
        tombstone.transform.position = transform.position;

        this.gameObject.SetActive(false);
    }

}
