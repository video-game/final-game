using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoProjectileScript : MonoBehaviour
{
	[SerializeField]
	private float lifetime;

	private void Awake()
	{
		StartCoroutine(DestroyIn(lifetime));
	}

	private IEnumerator DestroyIn(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		Destroy(gameObject);
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		Destroy(gameObject);
	}
}
