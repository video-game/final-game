using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
	[SerializeField]
	private Transform destination;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player" && !other.isTrigger)
		{
			if (destination == null)
			{
				Debug.Log("Teleporter destination not set.");
				return;
			}

			foreach (var player in GameManager.Instance.player)
				player.Teleport(destination.position);
			GameObject.FindObjectOfType<Camera>().transform.position = destination.position;
		}
	}
}
