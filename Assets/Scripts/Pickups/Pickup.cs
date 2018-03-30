using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pickup : MonoBehaviour
{
	protected Player player;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			player = other.GetComponent<Player>();
			Effect();
			Destroy(gameObject);
		}
	}

	abstract protected void Effect();
}
