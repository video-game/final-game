using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect : ScriptableObject {

    public virtual void Trigger() { }

    public virtual void Trigger(AbilityHitDetector hitWith, GameObject hit) { }

    public virtual void Trigger(AbilityHitDetector hitWith) { }

    public virtual void Trigger(Ability ability) { }

}
