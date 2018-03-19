using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemy : MonoBehaviour {

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
            Knockback(collision.gameObject.GetComponent<DemoProjectileScript>().velocity, 5);
            health -= 10;
            if(health < 1)
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void Knockback(Vector3 direction, float power)
    {
        //normalize the vector, just to be sure
        direction.Normalize();
        agent.velocity = new Vector3(direction.x, 0, direction.z) * power;
    }

    void Update ()
    {
        playerCheckTimer += Time.deltaTime;
        if (playerCheckTimer > playerCheckTime)
        {
            playerCheckTimer = 0;
            Vector3 playerPosition = GameManager.Instance.player.transform.position;
            NavMeshPath path = new UnityEngine.AI.NavMeshPath();

            //calculate the distance of path to player
            agent.CalculatePath(playerPosition, path);
            float distance = Utilities.PathDistance(path);
            if (attackDistance > distance)
            {
                inAttackDistance = true;
            }
            else if (stopDistance < distance)
            {
                inAttackDistance = false;
            }

            //If enemy is aggro, move towards him and try to shoot him (if he isn't behind a wall).
            if (inAttackDistance)
            {
                agent.SetDestination(playerPosition);

                Vector3 zeroedPos = new Vector3(transform.position.x, 0, transform.position.z);

                GameObject clone = Instantiate(bullet, new Vector3(transform.position.x, 0, transform.position.z), bullet.transform.rotation);
                clone.GetComponent<Rigidbody>().velocity = (playerPosition - zeroedPos).normalized * 5;


                //TODO make this work
                //for some reason raycasting on the mesh doesn't work so we just have to make it shoot at blockades I guess

                //The NavMeshAgent tends to set the y value to something not quite 0,
                //and the environment mesh is only exactly at y=0.
                //So we must fix this by raycasting a y zeroed vector.
                /*
                RaycastHit hit;
                Debug.DrawRay(zeroedPos, (playerPosition - zeroedPos), Color.red, playerCheckTime);
                
                if (Physics.Raycast(zeroedPos, (playerPosition - zeroedPos), out hit))
                {
                    Debug.Log("hit something! " + hit.transform);
                }
                */
            }
        }
	}

    
}
