using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tombstone : MonoBehaviour {

    SpriteRenderer renderer;
    public Sprite[] sprites;

    [SerializeField]
    int hitsToBreak = 2;
    int hits = 0;

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "PlayerProjectile")
        {
            if((float)++hits/(float)hitsToBreak >= 0.5f)
            {
                renderer.sprite = sprites[1];
            }
            if (hitsToBreak == hits)
            {
                renderer.sprite = sprites[2];
                GetComponent<Collider>().enabled = false;
            }
        }
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
