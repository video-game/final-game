using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [HideInInspector]
    public GameObject shooter;
    public GameObject Shooter { get { return shooter; } }

    public int damage = -10;
    
    //How inaccurate the projectile should be on a scale from 0 to 1
    public float inaccuracy = 0;
    public float speed = 1;
    [HideInInspector]
    public Vector3 velocity;

	[SerializeField]
	private float lifetime;

    [SerializeField]
    Sprite[] spriteList;

    [SerializeField]
    [Tooltip("The sound of the weapon, plays nothing if left empty.")]
    string projectileAudio = "DefaultWeapon";
    private Animator anim;

    public virtual void Init(Vector3 vel, GameObject shooter)
    {
        //set the height to 0.01 so it can detect collisions with mesh collider.
        transform.position = transform.position + new Vector3(0, 0.01f, 0);

        this.shooter = shooter;

        //Set the direction according to original velocity + accuracy
        float accuracyToDeg = (inaccuracy * 180);
        float randomAngle = Random.Range(-accuracyToDeg, accuracyToDeg);
        Quaternion randomRotation = Quaternion.AngleAxis(randomAngle, Vector3.up);
        vel = randomRotation * vel;
        vel = new Vector3(vel.x, 0, vel.z);
        vel.Normalize();
        velocity = vel * speed;
        GetComponent<Rigidbody>().velocity = velocity;

        float rot = Quaternion.LookRotation(velocity).eulerAngles.y;
        //rotate the projectile towards its direction
        transform.rotation = Quaternion.Euler(new Vector3(90, rot, 0));

        //Temporary fix for the bunny projectiles
        if (spriteList.Length != 0)
        {
            GetComponent<SpriteRenderer>().sprite = spriteList[Random.Range(0, spriteList.Length)];
        }

        if(projectileAudio != "")
            AudioManager.Instance.PlayAudioClip(projectileAudio, 10f);
    }

	private void Awake()
	{
        Destroy(gameObject, lifetime);

        //somewhere, init is not being called
        //so I guess we set animator here
        anim = GetComponent<Animator>();
    }

	public virtual void OnCollisionEnter(Collision other)
    {
         Explode();
	}
    
    //This will trigger an explosion in the animator (duh).
    //The animator will take care of actually destroying it
    //after the animation is finished.
    protected virtual void Explode()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Collider>().enabled = false;
        anim.SetTrigger("explode");
        
        Camera.main.GetComponent<CameraEffects>().ShakeCamera(0.08f, damage/800f);
    }
}
