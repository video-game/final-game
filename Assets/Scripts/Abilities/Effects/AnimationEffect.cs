using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/Effect/AnimationOnHit", fileName = "New Animation OnHit Effect")]
public class AnimationEffect : Effect
{
    public bool loop;

    public bool destroyOnAnimEnd;

    public bool stopMovementOnStart;

    public List<AnimEffect> animList;

    AbilityHitDetector hitDetector;

    public override void Trigger(AbilityHitDetector hitWith)
    {
        hitDetector = hitWith;
        ((RangedAbilityHitDetector)hitDetector).HitSlave.StopAllCoroutines();
        ((RangedAbilityHitDetector)hitDetector).HitSlave.StartCoroutine(PlayAnimation());
    }

    public override void Trigger(AbilityHitDetector hitWith, GameObject hitTarget)
    {
        hitDetector = hitWith;
        ((RangedAbilityHitDetector)hitDetector).HitSlave.StopAllCoroutines();
        ((RangedAbilityHitDetector)hitDetector).HitSlave.StartCoroutine(PlayAnimation());
    }

    protected IEnumerator PlayAnimation()
    {
        SpriteRenderer sr = hitDetector.GetComponent<SpriteRenderer>();

        if (stopMovementOnStart)
        {
            hitDetector.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }

        do
        {
            for (int i = 0; i < animList.Count; i++)
            {
                if (sr == null)
                {
                    break;
                }
                sr.sprite = animList[i].Sprite;
                yield return new WaitForSeconds(animList[i].duration);
                if(sr == null)
                {
                    break;
                }
            }

        } while (loop);

        if (sr == null != destroyOnAnimEnd)
        {
            hitDetector.Destroy();
        }
    }

}

[System.Serializable]
public struct AnimEffect
{
    public Sprite Sprite;
    [Range(0.1f, 3f)]
    public float duration;
}
