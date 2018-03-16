using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingEnemyAI : MonoBehaviour
{
	[SerializeField]
	private float startChasingRadius;

	[SerializeField]
	private float stopChasingRadius;

	private GameObject player;
	private ChargingEnemyMovement movement;
	//private GoblinCombatScript combat;
	private Rigidbody2D body;

	private bool playerWithinReach = false;
	private bool chasingPlayer = false;

	private void Awake()
	{
		player = GameObject.Find("Player");
		movement = GetComponent<ChargingEnemyMovement>();
		//combat = GetComponent<GoblinCombatScript>();
		body = GetComponent<Rigidbody2D>();
	}

	private void Update()
	{
		float distanceToPlayer = Vector2.Distance(player.transform.position, transform.position);

		if (!chasingPlayer && distanceToPlayer <= startChasingRadius)
			chasingPlayer = true;
		else if (distanceToPlayer > stopChasingRadius)
			chasingPlayer = false;

		if (!playerWithinReach && chasingPlayer)
			movement.MoveTowards(player);
		else if (!chasingPlayer)
			movement.Stop();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player")
		{
			playerWithinReach = true;
			movement.Stop();
			body.bodyType = RigidbodyType2D.Kinematic;
		}
	}

	private void OnTriggerStay2D(Collider2D other)
	{
        //Enemy should try to attack here

        //if (other.tag == "Player")
			//combat.Attack(other.gameObject);
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.tag == "Player")
		{
			playerWithinReach = false;
			body.bodyType = RigidbodyType2D.Dynamic;
		}
	}
}
