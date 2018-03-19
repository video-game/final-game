﻿using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
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
    private Rigidbody demoProjectile; // prefab

    private Animator animator;
    private Rigidbody body;
    private Transform hand;

    private bool dashOnCooldown;
    private bool isWalking;

    private NavMeshAgent agent;

    private Vector3 aimDirection;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        body = GetComponentInChildren<Rigidbody>();
        hand = transform.Find("Hand");
        agent = GetComponent<NavMeshAgent>();
    }

    //player move function
    public void Move(float horizontal, float vertical)
    {
        isWalking = (horizontal != 0 || vertical != 0);
        animator.SetBool("IsWalking", (horizontal != 0 || vertical != 0));

        Vector3 movePos = new Vector3(transform.position.x + horizontal, 0, transform.position.z + vertical);
        agent.SetDestination(movePos);
    }

    //player dash function
    public void Dash()
    {
        if (!dashOnCooldown)
        {
            StartCoroutine(DashCoroutine());
            if (dashCooldown > 0f)
                StartCoroutine(DashCooldown());
        }
    }

    private IEnumerator DashCoroutine()
    {
        Vector3 currentDirection = agent.velocity.normalized;
        agent.velocity = new Vector3(currentDirection.x, 0, currentDirection.z) * dashSpeed;
        yield return new WaitForSeconds(dashDuration);
        agent.velocity = Vector2.zero;
    }

    private IEnumerator DashCooldown()
    {
        dashOnCooldown = true;
        yield return new WaitForSeconds(dashCooldown);
        dashOnCooldown = false;
    }

    public void Shoot()
    {
        Rigidbody clone = (Rigidbody)Instantiate(demoProjectile, new Vector3(hand.position.x, 0, hand.position.z), demoProjectile.transform.rotation);
        clone.velocity = 9 * aimDirection;
    }

    public void AimInDirection(Vector3 direction)
    {
        aimDirection = direction;

        Quaternion rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, Quaternion.LookRotation(aimDirection, Vector3.up).eulerAngles.y,
            transform.rotation.eulerAngles.z);

        hand.rotation = rotation;

        animator.SetInteger("Direction", (int)Utilities.VectorToDirection(direction.x, direction.z));
    }
}