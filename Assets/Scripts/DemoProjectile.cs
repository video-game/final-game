using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoProjectile : MonoBehaviour
{

    public int damage = -10;

    [HideInInspector]
    public Vector3 velocity;

	[SerializeField]
	private float lifetime;

    [SerializeField]
    Sprite[] spriteList;

    private Animator anim;

    public virtual void init(Vector3 vel)
    {
        velocity = vel;
        GetComponent<Rigidbody>().velocity = velocity;

        //Temporary fix for the bunny projectiles
        if(spriteList.Length != 0)
        {
            GetComponent<SpriteRenderer>().sprite = spriteList[Random.Range(0, spriteList.Length)];
        }
    }

	private void Awake()
	{
        Destroy(gameObject, lifetime);

        //somewhere, init is not being called
        //so I guess we set animator here
        anim = GetComponent<Animator>();
    }

	public void OnCollisionEnter(Collision other)
    {
         Explode();
	}
    
    //This will trigger an explosion in the animator (duh).
    //The animator will take care of actually destroying it
    //after the animation is finished.
    private void Explode()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Collider>().enabled = false;
        anim.SetTrigger("explode");
    }
}
