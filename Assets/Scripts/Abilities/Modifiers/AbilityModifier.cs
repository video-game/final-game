using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityModifier : ScriptableObject {

    float damage;
    float cooldown;

    public List<Effect> OnUseEffect;
    public List<Effect> OnHitEffect;
}
