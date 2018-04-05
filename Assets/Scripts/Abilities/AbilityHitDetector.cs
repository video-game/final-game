using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class AbilityHitDetector : MonoBehaviour {

    [HideInInspector]
    public Ability ability;
    protected float lifetime;
    protected string targetTag;

    public virtual void Init(Ability a, string t, float lt = 5)
    {
        ability = a;
        lifetime = lt;
        targetTag = t;
        Destroy(gameObject, lt);
    }

    public virtual void OnCollisionEnter(Collision other)
    {
        if(ability.UsedBy.gameObject.tag != other.transform.tag)
        {
            ability.OnHit(this, other.gameObject);
        }
    }

    public void Destroy()
    {
        if(this != null)
        {
            Destroy(this.gameObject);
        }
    }

}
