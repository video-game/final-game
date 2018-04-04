using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebProjectile : Projectile
{
	[SerializeField]
	private float slowPercentage;

	[SerializeField]
	private float slowDuration;

	public override void OnCollisionEnter(Collision other)
	{
		base.OnCollisionEnter(other);

		if (other.gameObject.tag == "Player")
		{
			var player = other.gameObject.GetComponent<Player>();
			player.Slow(slowPercentage, slowDuration);
		}
	}
}
