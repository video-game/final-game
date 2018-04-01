using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ProjectileAnimationHelper : MonoBehaviour {

    SpriteRenderer sR;

    public List<Sprite> explosion;

    private void Start()
    {
        sR = GetComponent<SpriteRenderer>();
    }

    [ExecuteInEditMode]
    public void Rotate(float degrees)
    {
        transform.Rotate(new Vector3(0, 0, degrees));
    }

    [ExecuteInEditMode]
    public void Explosion(int index)
    {
        if(index < explosion.Count)
        {
            sR.sprite = explosion[index];
        }
        else
        {
            //if index is out of range, chances are
            //there is no explosion animation.
            //Therefore we destroy it.
            Destroy();
        }
    }

    public void Destroy()
    {
        Destroy(this.gameObject);        
    }
}
