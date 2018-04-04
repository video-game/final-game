using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour {

    public string musicName;

    private bool entered = false;

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            if(!entered)
            {
                entered = true;
                var enemySpawners = transform.GetChild(0).transform.GetComponentsInChildren<EnemySpawner>();
                foreach (EnemySpawner spawner in enemySpawners)
                {
                    spawner.Spawn();
                }
                AudioManager.Instance.PlayMusicClip(musicName);
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (entered)
            {
                entered = false;
                var enemySpawners = transform.GetChild(0).transform.GetComponentsInChildren<EnemySpawner>();
                foreach (EnemySpawner spawner in enemySpawners)
                {
                    spawner.Clear();
                }
            }
        }
    }
}
