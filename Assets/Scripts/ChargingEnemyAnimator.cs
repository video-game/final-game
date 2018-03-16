using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingEnemyAnimator : MonoBehaviour
{
	private Animator animator;
	private Rigidbody2D body;

	private void Awake()
	{
		animator = GetComponent<Animator>();
		body = GetComponent<Rigidbody2D>();
	}

	public void AnimateMovement()
	{
		SetIsWalking();
		SetFacingDirection();
	}

	private void SetIsWalking()
	{
		bool isWalking = (body.velocity != Vector2.zero);
		animator.SetBool("IsWalking", isWalking);
	}

	private void SetFacingDirection()
	{
		Vector2 movingDirection = body.velocity.normalized;

		Utilities.Direction? facingDirection = null;

		if (movingDirection.x < 0)
			facingDirection = Utilities.Direction.Left;
		else if (movingDirection.x > 0)
			facingDirection = Utilities.Direction.Right;
		
		if (movingDirection.y < -0.75f)
			facingDirection = Utilities.Direction.Down;
		else if (movingDirection.y > 0.75)
			facingDirection = Utilities.Direction.Up;
		
		if (facingDirection.HasValue)
			animator.SetInteger("FacingDirection", (int)facingDirection.Value);
	}

	public void AnimateAttack()
	{
		animator.SetTrigger("Attack");
	}
}
