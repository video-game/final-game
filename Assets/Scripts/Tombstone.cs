using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tombstone : MonoBehaviour {

    SpriteRenderer renderer;
    public Sprite[] sprites;
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
            hits++;
            renderer.sprite = sprites[hits];
            if (hitsToBreak == hits)
            {
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
