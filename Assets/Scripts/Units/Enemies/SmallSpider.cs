using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SmallSpider : Enemy
{
	[SerializeField]
	private float attackDistance;

	[SerializeField]
	private float updatePathInterval;

	// Determines whether spider can damage player
	private bool harmful = true;

	protected override void Awake()
	{
		base.Awake();
		attacking = true;
	}

	private void Start()
	{
		StartCoroutine(ChasePlayer());
	}
	
	private void OnTriggerStay(Collider other)
	{
		if (harmful && other.tag == "Player")
		{
			var player = other.GetComponent<Player>();
			player.TakeTrueDamage(gameObject, -2 * GameManager.Instance.player.Count);
			harmful = false;
			StartCoroutine(harmfulInSeconds(0.5f));
		}
	}

	private IEnumerator harmfulInSeconds(float time)
	{
		yield return new WaitForSeconds(time);
		harmful = true;
	}
	
	protected override void Hit(Projectile projectile)
	{
		base.Hit(projectile);

		Knockback(projectile.velocity, Mathf.Abs(projectile.damage));
	}

	// Look for player every updatePathInterval seconds and chase him if within attackDistance
	private IEnumerator ChasePlayer()
	{
		// Keep looking until own game object is destroyed
		while (true)
		{
			// If player exists
			if (GameManager.Instance.player != null && GameManager.Instance.player.Count > 0)
			{
                Vector3 playerPosition = GetClosestPlayer();

                var path = new NavMeshPath();
				agent.CalculatePath(playerPosition, path);

				var distance = Utilities.PathDistance(path);

				if (distance <= attackDistance)
					attacking = true;
				
				if (attacking)
					agent.SetPath(path);
			}

			// Wait for updatePathInterval seconds before looking again
			yield return new WaitForSeconds(updatePathInterval);
		}
	}
}
