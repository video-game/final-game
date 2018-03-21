using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{

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
}
