using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossBunny : Enemy
{
	[SerializeField]
	private float range;

	[SerializeField]
	private float updatePathInterval;

	[SerializeField]
	private Rigidbody demoProjectile;

	private bool inRange, active;

    [SerializeField]
    private GameObject deathTeleporter;

    //How much time should pass until the bunny starts moving in another direction
    [SerializeField]
    private float movementDecisionTime;
    private float MDTimer = 0;

    //How far the bunny should be able to walk from its original spot
    [SerializeField]
    private float movementRadius;
    private Vector3 originalPos;

    private Color bodyColor;

	protected override void Awake()
	{
        base.Awake();

        bodyColor = transform.Find("Model - X Rotation at 90").GetComponent<SpriteRenderer>().color;
        originalPos = transform.position;

        OnDeath += delegate
        {
            if (deathTeleporter != null)
                deathTeleporter.SetActive(true);
        };
    }

    private void Start()
    {
        StartCoroutine(CheckPlayerRange());
    }

    private void Update()
	{
        
        animator.SetBool("IsWalking", agent.velocity != Vector3.zero);

        MDTimer += Time.deltaTime;
        
        if(inRange && !active)
        {
            active = true;
            StartCoroutine(AttackPlayer());
        }
	}

	private IEnumerator AttackPlayer() {
        Vector3 newPos = originalPos + new Vector3(Random.Range(0, movementRadius), 0, Random.Range(0, movementRadius));
        agent.SetDestination(newPos);
        while (true)
        {

            if (MDTimer > movementDecisionTime)
            {
                MDTimer = 0;
                newPos = originalPos + new Vector3(Random.Range(0, movementRadius), 0, Random.Range(0, movementRadius));
                agent.SetDestination(newPos);
            }

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
                    clone.GetComponent<Projectile>().Init(7.5f * direction, gameObject);
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
                Vector3 playerPosition = GetClosestPlayer();

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
			    clone.GetComponent<Projectile>().Init( 7.5f * direction, gameObject);
            }
            yield return new WaitForSeconds(0.1f);
		}
    }

	protected override void OnCollisionEnter(Collision other)
	{
        base.OnCollisionEnter(other);
        
		if (other.transform.tag == "PlayerProjectile")
        {
            Damaged();

            if (!active)
            {
                active = true;
                StartCoroutine(AttackPlayer());
            }
        }
	}

    public void Damaged()
    {
        animator.SetBool("IsHurt", true);
        StartCoroutine(DamagedCoroutine(0.4f));
    }

    //coroutine for when player is damaged, colors the player red for some time
    IEnumerator DamagedCoroutine(float duration)
    {
        float colorIntensity = 10;

        var modelRenderer = transform.Find("Model - X Rotation at 90").GetComponent<SpriteRenderer>();

        modelRenderer.color = new Color(bodyColor.r, bodyColor.g - colorIntensity, bodyColor.b - colorIntensity);

        yield return new WaitForSeconds(duration);

        animator.SetBool("IsHurt", false);
        modelRenderer.color = bodyColor;
    }
}
