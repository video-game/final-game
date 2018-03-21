using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    public delegate void StatChangeDelegate(int current, int max);
    public StatChangeDelegate OnHealthChange;

    protected int maxHealth;
    protected int currentHealth;

    public void ChangeHealth(int value)
    {
        currentHealth = (currentHealth + value) < 0 ? 0 : (currentHealth + value) > maxHealth ? maxHealth : (currentHealth + value);

        Debug.Log(currentHealth);

        if (OnHealthChange != null)
        {
            OnHealthChange(currentHealth, maxHealth);
        }
    }

}
