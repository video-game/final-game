using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyProjectile : PlayerProjectile {
    public override void OnCollisionEnter(Collision other)
    {
        Debug.Log(other.collider.name);
        if(other.collider.gameObject.tag == "Enemy")
        {
            transform.SetParent(other.collider.transform);
        }

        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Collider>().enabled = false;
    }
}
