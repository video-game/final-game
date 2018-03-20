using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BomberEnemy : MonoBehaviour
{
	[SerializeField]
	private float health;
	public float Health { get { return health; } }

	[SerializeField]
	private float attackDistance;

	[SerializeField]
	private float stopDistance;

	[SerializeField]
	private float updatePathInterval;

	[SerializeField]
	private Rigidbody demoProjectile;

	private NavMeshAgent agent;
	private Animator animator;

	private bool inAttackDistance;

	private void Awake()
	{
		agent = GetComponent<NavMeshAgent>();
		animator = transform.GetComponentInChildren<Animator>();
	}

	private void Start()
	{
		StartCoroutine(ChasePlayer());
	}

	private void Update()
	{
		if (health <= 0)
			die();

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
				var playerPosition = GameManager.Instance.player[0].transform.position;

				var path = new NavMeshPath();
				agent.CalculatePath(playerPosition, path);

				var distance = Utilities.PathDistance(path);

				if (distance <= attackDistance)
					inAttackDistance = true;
				else if (distance >= stopDistance)
					inAttackDistance = false;
				
				if (inAttackDistance)
				{
					agent.SetPath(path);
					agent.isStopped = false;
				}
				else
					agent.isStopped = true;
			}

			// Wait for updatePathInterval seconds before looking again
			yield return new WaitForSeconds(updatePathInterval);
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		if (other.transform.tag == "Player")
			health = 0;
		else if (other.transform.tag == "PlayerProjectile")
			health -= 10;
	}

	private void die()
	{
		explode();
        var effects = Camera.main.GetComponent<CameraEffects>();
        if(effects != null)
        {
            effects.ShakeCamera(0.2f, 0.05f);
        }
        Destroy(gameObject);
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
				clone.GetComponent<DemoProjectile>().init( 7.5f * direction);
			}
		}
	}
}
