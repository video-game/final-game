using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingEnemyMovement : MonoBehaviour
{
	[SerializeField]
	private float speed;

	private Rigidbody2D body;
	private ChargingEnemyAnimator animatorScript;

	private void Awake()
	{
		body = GetComponent<Rigidbody2D>();
		animatorScript = GetComponent<ChargingEnemyAnimator>();
	}

	public void MoveTowards(GameObject target)
	{
		Vector2 goblinPosition = transform.position;
		Vector2 targetPosition = target.transform.position;
		Vector2 movingDirection = (targetPosition - goblinPosition).normalized;
		Vector2 velocity = movingDirection * speed;
		body.velocity = velocity;

		animatorScript.AnimateMovement();
	}

	public void Stop()
	{
		body.velocity = Vector2.zero;
		animatorScript.AnimateMovement();
	}
}
