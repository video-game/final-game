using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
	[SerializeField]
	private float speed;

	[SerializeField]
	private float handRadius;

	[SerializeField]
	private Vector2 centerpointOffset;

	[SerializeField]
	private Rigidbody2D demoProjectile; // prefab

	private Animator animator;
	private Rigidbody2D body;
	private Transform hand;

	private bool fireKeyDown;

	private void Awake()
	{
		animator = GetComponent<Animator>();
		body = GetComponent<Rigidbody2D>();
		hand = transform.Find("Hand");
	}

	private void Update()
	{
		move();
		faceTowardsMouse();
		moveHandTowardsMouse();

		if (Input.GetAxisRaw("Fire1") != 0)
		{
			if (!fireKeyDown)
			{
				shootDemoProjectile();
				fireKeyDown = true;
			}
		}
		else
			fireKeyDown = false;
	}

	private void shootDemoProjectile()
	{
		var direction = calculateDirectionFromPlayerToMouse();
		var clone = Instantiate(demoProjectile, hand.position, Quaternion.identity);
		clone.velocity = 9 * direction;
	}

	private void move()
	{
		var horizontalInput = Input.GetAxisRaw("Horizontal");
		var verticalInput = Input.GetAxisRaw("Vertical");

		var velocity = new Vector2();
		velocity.x = horizontalInput * speed;
		velocity.y = verticalInput * speed;

		// if moving diagonally
		if (horizontalInput != 0 && verticalInput != 0)
			velocity *= Mathf.Cos(45 * Mathf.Deg2Rad);
		
		body.velocity = velocity;

		var isWalking = (horizontalInput != 0 || verticalInput != 0);
		animator.SetBool("IsWalking", isWalking);
	}

	private void faceTowardsMouse()
	{
		var directionVector = calculateDirectionFromPlayerToMouse();
		var x = directionVector.x;
		var y = directionVector.y;

		Utilities.Direction direction;

		if (x < 0 && Mathf.Abs(x) > Mathf.Abs(y))
			direction = Utilities.Direction.Left;
		else if (x > 0 && x > Mathf.Abs(y))
			direction = Utilities.Direction.Right;
		else if (y > 0 && y > Mathf.Abs(x))
			direction = Utilities.Direction.Up;
		else
			direction = Utilities.Direction.Down;

		animator.SetInteger("Direction", (int)direction);
	}

	private Vector2 calculateDirectionFromPlayerToMouse()
	{
		var mousePosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
		var playerPosition = (Vector2)transform.position + centerpointOffset;
		var direction = (mousePosition - playerPosition).normalized;
		return direction;
	}

	private void moveHandTowardsMouse()
	{
		var playerPosition = (Vector2)transform.position + centerpointOffset;
		var direction = calculateDirectionFromPlayerToMouse();
		var handPosition = playerPosition + (direction * handRadius);
		hand.position = handPosition;
	}
}
