using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossBunny : MonoBehaviour
{
	[SerializeField]
	private float health;
	public float Health { get { return health; } }

	[SerializeField]
	private float range;

	[SerializeField]
	private float updatePathInterval;

	[SerializeField]
	private Rigidbody demoProjectile;

	private NavMeshAgent agent;
    //private Animator animator;

	private bool inRange, active;

	private void Awake()
	{
		agent = GetComponent<NavMeshAgent>();
        //animator = transform.GetComponentInChildren<Animator>();
	}

	private void Start()
	{
		StartCoroutine(CheckPlayerRange());
	}

	private void Update()
	{
        //animator.SetBool("IsRunning", inRange);
        if(inRange && !active)
        {
            active = true;
            StartCoroutine(AttackPlayer());
        }

        if (health <= 0)
			die();
	}

	private IEnumerator AttackPlayer() {
        while(true)
        {
            switch (Random.Range(0, 2))
            {
                case 0:
                    StartCoroutine(SpiralAttack());
                    break;
                case 1:
                    StartCoroutine(BombAttack());
                    break;
            }
            yield return new WaitForSeconds(4);
        }
    }

    private IEnumerator BombAttack()
    {
        for (int shots = 0; shots <= 7; shots++)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    if (x == 0 && z == 0)
                        continue;

                    var direction = new Vector3(x, 0, z).normalized;
                    var clone = Instantiate(demoProjectile, transform.position, demoProjectile.transform.rotation);
                    clone.velocity = 7.5f * direction;
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

	// Look for player every updatePathInterval seconds and chase him if within attackDistance
	private IEnumerator CheckPlayerRange()
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

				inRange = (distance <= range);
			}

			// Wait for updatePathInterval seconds before looking again
			yield return new WaitForSeconds(updatePathInterval);
		}
	}

    private IEnumerator SpiralAttack()
    {
		for (int angle = 0; angle <= 50; angle += 2)
		{
            for (int arm = 1; arm <= 360; arm+= 360/6){
			    var direction = new Vector3(Mathf.Cos(angle + arm * Mathf.Deg2Rad), 0, Mathf.Sin(angle + arm * Mathf.Deg2Rad));
			    var clone = Instantiate(demoProjectile, transform.position, demoProjectile.transform.rotation);
			    clone.velocity = 7.5f * direction;
            }
            yield return new WaitForSeconds(0.1f);
		}
    }

	private void OnCollisionEnter(Collision other)
	{
		if (other.transform.tag == "PlayerProjectile")
			health -= 10;
	}

	private void die()
	{
		Destroy(gameObject);
	}
}
