using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {

    [SerializeField]
    private float playerCheckTime;
    private float playerCheckTimer = 0;

    NavMeshAgent agent;

    [SerializeField]
    private float attackDistance = 4;
    [SerializeField]
    private float stopDistance = 6;

    private bool inAttackDistance = false;

    public GameObject bullet;

    public float health;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "PlayerProjectile")
        {
            Knockback(collision.gameObject.GetComponent<Rigidbody>().velocity.normalized, 5);
            health -= 10;
            if(health < 1)
            {
                Destroy(this.gameObject);
            }

        }
    }

    private void Knockback(Vector3 direction, float power)
    {
        agent.velocity = direction * power;
    }
}
