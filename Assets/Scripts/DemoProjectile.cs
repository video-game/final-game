using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoProjectile : MonoBehaviour
{
    [HideInInspector]
    public Vector3 velocity;

	[SerializeField]
	private float lifetime;

    public void init(Vector3 vel)
    {
        velocity = vel;
        GetComponent<Rigidbody>().velocity = velocity;
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
