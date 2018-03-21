using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoProjectile : MonoBehaviour
{
    [HideInInspector]
    public Vector3 velocity;

	[SerializeField]
	private float lifetime;

    [SerializeField]
    Sprite[] spriteList;
    public void init(Vector3 vel)
    {
        velocity = vel;
        GetComponent<Rigidbody>().velocity = velocity;

        //Temporary fix for the bunny projectiles
        if(spriteList.Length != 0)
        {
            GetComponent<SpriteRenderer>().sprite = spriteList[Random.Range(0, spriteList.Length)];
        }
    }

	private void Awake()
	{
        Destroy(gameObject, lifetime);
	}

	public void OnCollisionEnter(Collision other)
    {
		Destroy(gameObject);
	}
}
