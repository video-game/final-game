    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpiderBoss : Enemy
{
	[SerializeField]
    private float attackDistance;

    [SerializeField]
    private float timeBetweenPlayerSearches;
    
    [SerializeField]
    private float movementRadius;

    [SerializeField]
    private float timeBetweenSteps;

    [SerializeField]
    private float timeBetweenAttacks;

    [SerializeField]
    private float timeBetweenProjectiles;

    [SerializeField]
    private int numberOfProjectilesPerBarragePerPlayer;

    [SerializeField]
    private List<EggBatch> eggBatches;

    [SerializeField]
    private GameObject projectile;

    [SerializeField]
    private GameObject deathTeleporter;

    private Vector3 initialPosition;
    private Color initialColor;
    private bool attackingLastFrame;

    protected override void Awake()
    {
        base.Awake();
        initialColor = transform.GetComponentInChildren<SpriteRenderer>().color;
        initialPosition = transform.position;

        OnDeath += delegate
        {
            foreach (var p in GameManager.Instance.player)
                if (p != lastAttacker.GetComponent<Player>())
                    p.GrantExperience(experienceAward);

            if (deathTeleporter != null)
                deathTeleporter.SetActive(true);
        };
    }

	private void Start()
	{
		StartCoroutine(SearchForPlayer());
	}

    private void Update()
    {
        animator.SetBool("IsWalking", agent.velocity != Vector3.zero);
        
        if (attacking && !attackingLastFrame)
        {
            StartCoroutine(MoveAround());
            StartCoroutine(Attack());
            attackingLastFrame = true;
        }

        if (attacking)
            foreach (var batch in eggBatches)
                if (!batch.Hatched && (currentHealth / (float)MaxHealth) <= batch.hatchAtPercentage)
                    batch.Hatch();
    }

    private IEnumerator Attack()
    {
        while (attacking)
        {
            var alivePlayerIndices = new List<int>();
            for (var i = 0; i < GameManager.Instance.player.Count; i++)
                if (GameManager.Instance.player[i].Alive)
                    alivePlayerIndices.Add(i);
            if (alivePlayerIndices.Count > 0)
            {
                for (var i = 0; i < numberOfProjectilesPerBarragePerPlayer * GameManager.Instance.player.Count; i++)
                {
                    var targetIndex = Random.Range(0, alivePlayerIndices.Count);
                    var target = GameManager.Instance.player[targetIndex];

                    var clone = Instantiate(projectile, transform.position, transform.rotation);
                    var direction = (target.transform.position - transform.position).normalized;
                    clone.GetComponent<Projectile>().Init(direction, gameObject);

                    yield return new WaitForSeconds(timeBetweenProjectiles);
                }
            }
            else
                attacking = false;

            yield return new WaitForSeconds(timeBetweenAttacks);
        }
    }

    private IEnumerator MoveAround()
    {
        while (attacking)
        {
            var x = Random.Range(-movementRadius, movementRadius);
            var z = Random.Range(-movementRadius, movementRadius);
            var destination = initialPosition + new Vector3(x, 0, z);
            agent.SetDestination(destination);
            yield return new WaitForSeconds(timeBetweenSteps);
        }
    }

	private IEnumerator SearchForPlayer()
	{
		while (!attacking)
		{
			if (GameManager.Instance.player != null && GameManager.Instance.player.Count > 0)
			{
                var playerPosition = GetClosestPlayer();

                var path = new NavMeshPath();
				agent.CalculatePath(playerPosition, path);

				var distance = Utilities.PathDistance(path);

				if (distance <= attackDistance)
					attacking = true;
			}

			yield return new WaitForSeconds(timeBetweenPlayerSearches);
		}
	}

    protected override void OnCollisionEnter(Collision collision)
    {
        
        if (collision.transform.tag == "PlayerProjectile")
        {
            AudioManager.Instance.PlayAudioClip("BossDamage");
        }
        base.OnCollisionEnter(collision);
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}

[System.Serializable]
public class EggBatch
{
    public float hatchAtPercentage;
    public List<SpiderEgg> eggs;

    private bool hatched;
    public bool Hatched { get { return hatched; } }

    public void Hatch()
    {
        hatched = true;
        foreach (var egg in eggs)
            egg.Hatch();
    }
}
