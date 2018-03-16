
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoProjectileScript : MonoBehaviour
{
	[SerializeField]
	private float lifetime;

	private void Awake()
	{
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), GameManager.Instance.player.GetComponent<Collider2D>());
		StartCoroutine(DestroyIn(lifetime));
	}

	private IEnumerator DestroyIn(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		Destroy(gameObject);
	}

	private void OnCollisionEnter2D(Collision2D other)
    {   
		Destroy(gameObject);
	}
}

