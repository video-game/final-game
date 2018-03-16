
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoProjectileScript : MonoBehaviour
{
	[SerializeField]
	private float lifetime;

	private void Awake()
	{
        Destroy(gameObject, lifetime);
	}

	private void OnCollisionEnter2D(Collision2D other)
    {   
		Destroy(gameObject);
	}
}

