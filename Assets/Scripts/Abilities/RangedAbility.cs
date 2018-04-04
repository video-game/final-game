using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/Ranged", fileName = "New Ranged Ability")]
public class RangedAbility : Ability {

    public Sprite projectileSprite;
    public float speed = 8;
    public float spread = 10;
    public float lifeTime = 10;
    public float projectileCount = 1;

    public override void Init(Unit u)
    {
        base.Init(u);
        spread = spread + (projectileCount * 10);
    }

    public override void OnUse(AbilityHitDetector AHD = null)
    {

        if (ready)
        {
            for (int i = 0; i < projectileCount; i++)
            {
                RangedAbilityHitDetector temp = new GameObject().AddComponent<RangedAbilityHitDetector>();
                temp.Init(this, "Enemy", UsedBy.AimDirection, lifeTime);

                base.OnUse(temp);
            }
           
        }

    }

}
