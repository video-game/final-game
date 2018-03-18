using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
	[SerializeField]
	private float speed;

	[SerializeField]
	private float dashSpeed;

	[SerializeField]
	private float dashDuration;

	[SerializeField]
	private float dashCooldown;

	[SerializeField]
	private float handRadius;

	[SerializeField]
	private Vector2 centerpointOffset;

	[SerializeField]
	private Rigidbody demoProjectile; // prefab

	private Animator animator;
	private Rigidbody body;
	private Transform hand;

	private bool fireKeyDown;
	private bool dashKeyDown;
	private bool dashOnCooldown;
	private bool movementInputEnabled = true;
	private bool isWalking;

	private void Awake()
	{
		animator = GetComponent<Animator>();
		body = GetComponent<Rigidbody>();
		hand = transform.Find("Hand");
	}

	private void Update()
	{
		handleMovementInput();
		handleDashInput();
		handleFireInput();
		faceTowardsMouse();
		moveHandTowardsMouse();
	}

	private void handleFireInput()
	{
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

	private void handleDashInput()
	{
		if (Input.GetAxis("Dash") != 0)
		{
			if (!dashKeyDown)
			{
				if (!dashOnCooldown)
				{
					StartCoroutine(dash());
					if (dashCooldown > 0f)
						StartCoroutine(startDashCooldown());
				}
				dashKeyDown = true;
			}
		}
		else
			dashKeyDown = false;
	}

	private IEnumerator dash()
	{
		if (isWalking)
		{
			movementInputEnabled = false;
			Vector2 currentDirection = body.velocity.normalized;
			body.velocity = new Vector3(currentDirection.x, 0 , currentDirection.y) * dashSpeed;
			yield return new WaitForSeconds(dashDuration);
			body.velocity = Vector2.zero;
			movementInputEnabled = true;
		}
	}

	private IEnumerator startDashCooldown()
	{
		dashOnCooldown = true;
		yield return new WaitForSeconds(dashCooldown);
		dashOnCooldown = false;
	}

	private void shootDemoProjectile()
	{
		var direction = calculateDirectionToMouse();
		var clone = Instantiate(demoProjectile, new Vector3(hand.position.x, 0, hand.position.z), demoProjectile.transform.rotation);
		clone.velocity = 9 * direction;
	}

	private void handleMovementInput()
	{
		if (movementInputEnabled)
		{
			var horizontalInput = Input.GetAxisRaw("Horizontal");
			var verticalInput = Input.GetAxisRaw("Vertical");

			var velocity = new Vector3();
			velocity.x = horizontalInput * speed;
			velocity.z = verticalInput * speed;

			// if moving diagonally
			if (horizontalInput != 0 && verticalInput != 0)
				velocity *= Mathf.Cos(45 * Mathf.Deg2Rad);
			
			body.velocity = velocity;

			isWalking = (horizontalInput != 0 || verticalInput != 0);
			animator.SetBool("IsWalking", isWalking);
		}
	}

	private void faceTowardsMouse()
	{
		var directionVector = calculateDirectionToMouse();
		var x = directionVector.x;
		var z = directionVector.z;

		Utilities.Direction direction;

		if (x < 0 && Mathf.Abs(x) > Mathf.Abs(z))
			direction = Utilities.Direction.Left;
		else if (x > 0 && x > Mathf.Abs(z))
			direction = Utilities.Direction.Right;
		else if (z > 0 && z > Mathf.Abs(x))
			direction = Utilities.Direction.Up;
		else
			direction = Utilities.Direction.Down;

		animator.SetInteger("Direction", (int)direction);
	}

	private Vector3 calculateDirectionToMouse()
	{
		var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		var playerPosition = transform.position + new Vector3(centerpointOffset.x, 0, centerpointOffset.y);
		var direction = (mousePosition - playerPosition).normalized;
		return direction;
	}

	private void moveHandTowardsMouse()
	{
		Vector3 playerPosition = transform.position + new Vector3(centerpointOffset.x, 0, centerpointOffset.y);
		Vector3 direction = calculateDirectionToMouse();
		var handPosition = playerPosition + (direction * handRadius);
		hand.position = handPosition;
	}
}
