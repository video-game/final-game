using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BomberEnemy : Enemy
{
	[SerializeField]
	private float attackDistance;

	[SerializeField]
	private float updatePathInterval;

	[SerializeField]
	private Rigidbody demoProjectile;

	public override void Awake()
	{
        base.Awake();

		OnDeath += die;
	}

	private void Start()
	{
		StartCoroutine(ChasePlayer());
	}

	private void Update()
	{
		animator.SetBool("IsRunning", agent.velocity != Vector3.zero);
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

	private void OnCollisionEnter(Collision other)
	{
		if (other.transform.tag == "Player")
			ChangeHealth(-maxHealth);
		else if (other.transform.tag == "PlayerProjectile")
		{
			ChangeHealth(-10);
			damageTakenCanvas.InitializeDamageText(10.ToString());

			attacking = true;
		}
	}

	// Called in grandparent Unit.Dead() method
	private void die()
	{
		explode();

        var effects = Camera.main.GetComponent<CameraEffects>();
        if (effects != null)
            effects.ShakeCamera(0.2f, 0.05f);

        var tombstone = Instantiate(GameManager.Instance.Tombstone);
        tombstone.transform.position = transform.position;

		damageTakenCanvas.Orphan();
	}

	private void explode()
	{
		// Fire projectiles in 8 directions (Left, Down, Right, Up, and between)
		for (int x = -1; x <= 1; x++)
		{
			for (int z = -1; z <= 1; z++)
			{
				if (x == 0 && z == 0)
					continue;
				
				var direction = new Vector3(x, 0, z).normalized;
				var clone = Instantiate(demoProjectile, transform.position, demoProjectile.transform.rotation);
				clone.GetComponent<DemoProjectile>().init(7.5f * direction);
			}
		}
	}
}
