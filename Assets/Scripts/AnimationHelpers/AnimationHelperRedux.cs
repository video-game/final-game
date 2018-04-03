using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Remade because the other one was hard to work with.
//Still using the old one though, as switching it out 
//for all users of it is a nuisance.
[RequireComponent(typeof(SpriteRenderer))]
public class AnimationHelperRedux : MonoBehaviour
{

    SpriteRenderer sR;
    public List<Sprite> idle;

    [System.Serializable]
    public class ActionAnimation
    {
        public List<Sprite> left;
        public List<Sprite> right;
        public List<Sprite> down;
        public List<Sprite> up;
    }
    public ActionAnimation walk;
    public ActionAnimation attack;
    public List<Sprite> ko;

    private void Start()
    {
        sR = GetComponent<SpriteRenderer>();
    }

    [Tooltip("How many steps there are in the walking animation")]
    public int walkStep;
    private int walkStepCount = 0;

    [Tooltip("How many steps there are in the attack animation")]
    public int attackStep;
    private int attackStepCount = 0;

    [ExecuteInEditMode]
    public void Walk(Utilities.Direction direction)
    {
        attackStepCount = 0;
        switch (direction)
        {
            case (Utilities.Direction.Left):
                sR.sprite = walk.left[walkStepCount];
                break;
            case (Utilities.Direction.Right):
                sR.sprite = walk.right[walkStepCount];
                break;
            case (Utilities.Direction.Up):
                sR.sprite = walk.up[walkStepCount];
                break;
            case (Utilities.Direction.Down):
                sR.sprite = walk.down[walkStepCount];
                break;
            default:
                sR.sprite = idle[0];
                break;
        }
        walkStepCount = (walkStepCount + 1)%walkStep;

    }

    [ExecuteInEditMode]
    public void Attack(Utilities.Direction direction)
    {
        walkStepCount = 0;
        switch (direction)
        {
            case (Utilities.Direction.Left):
                sR.sprite = attack.left[attackStepCount];
                break;
            case (Utilities.Direction.Right):
                sR.sprite = attack.right[attackStepCount];
                break;
            case (Utilities.Direction.Up):
                sR.sprite = attack.up[attackStepCount];
                break;
            case (Utilities.Direction.Down):
                sR.sprite = attack.down[attackStepCount];
                break;
            default:
                sR.sprite = idle[0];
                break;
        }

        attackStepCount = (attackStepCount + 1) % attackStep;

    }

    [ExecuteInEditMode]
    public void Idle(Utilities.Direction direction)
    {
        sR.sprite = idle[0];
    }

    [ExecuteInEditMode]
    public void KO(int state)
    {
        sR.sprite = ko[state];
    }
}
