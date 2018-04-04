using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/Ranged", fileName = "New Ranged Ability")]
public class RangedAbility : Ability {

    public Sprite projectileSprite;
    public float speed;
    public float spread;
    public float lifeTime;

    [HideInInspector]
    public RangedAbilityHitDetector RAHD;

    public override void Init(Unit u)
    {
        base.Init(u);
    }

    public void OnUse()
    {
        if (ready)
        {
            GameObject temp = new GameObject();
            RAHD = temp.AddComponent<RangedAbilityHitDetector>();
            RAHD.Init(this, "Enemy", UsedBy.AimDirection, lifeTime);
            
            base.OnUse(RAHD);
        }
    }

}
