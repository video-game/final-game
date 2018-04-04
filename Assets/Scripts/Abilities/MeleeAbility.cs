using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/Melee", fileName = "New Melee Ability")]
public class MeleeAbility : Ability {

    public float distance;
    public float degrees;
    public MeleeAbilityHitDetector MAHD;

    public void OnUse()
    {
        MeleeAbilityHitDetector MAHD = new GameObject().AddComponent< MeleeAbilityHitDetector>();
        MAHD.Init(this, "Enemy" , UsedBy.AimDirection);
        base.OnUse(MAHD);
    }

}
