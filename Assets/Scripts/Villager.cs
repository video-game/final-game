using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : MonoBehaviour {

    private SpriteRenderer renderer;
    [SerializeField]
    private Sprite[] directions;
    [SerializeField]
    private float viewDistance;
	// Use this for initialization
	void Awake () {
        renderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 playerPos = GetClosestPlayer();

        if (Vector3.Distance(transform.position, playerPos) < viewDistance)
        {
            Vector3 directionVector = playerPos - transform.position;
            renderer.sprite = directions[(int)Utilities.VectorToDirection(directionVector.x, directionVector.z)];
        }
        else
        {
            //not really sure if it's bad to set the sprite every frame for villager (i'm hoping no)
            renderer.sprite = directions[3];
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
}
