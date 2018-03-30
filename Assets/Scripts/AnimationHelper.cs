using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AnimationHelper : MonoBehaviour {

    SpriteRenderer sR;

    public List<Sprite> idle;
    public List<Sprite> walk;
    public List<Sprite> ko;

    private void Start()
    {
        sR = GetComponent<SpriteRenderer>();
    }

    [ExecuteInEditMode]
    public void Walk(Utilities.Direction direction)
    {
        sR.sprite = walk[(int)direction];
    }

    [ExecuteInEditMode]
    public void Idle(Utilities.Direction direction)
    {
        sR.sprite = idle[(int)direction];
    }

    [ExecuteInEditMode]
    public void KO(int state)
    {
        sR.sprite = ko[state];
    }
}
