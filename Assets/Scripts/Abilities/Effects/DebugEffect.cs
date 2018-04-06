using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/Effect/DebugTextEffect", fileName = "New Debug Effect")]
public class DebugOnHitEffect : Effect {

    public string debugText;

    public override void Trigger(AbilityHitDetector hitWith, GameObject hit)
    {
        Debug.Log(debugText);
    }

}
