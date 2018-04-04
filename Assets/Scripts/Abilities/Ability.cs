using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : ScriptableObject {

    [HideInInspector]
    public Unit UsedBy;
    [HideInInspector]
    public CoroutineSlave slave;
    public UnityEngine.UI.Image AbilityBarImage;
    public float damage;
    public float cooldown;
    [HideInInspector]
    public bool ready;
    public List<Effect> OnUseEffect;
    public List<Effect> OnHitEffect;

    private List<Effect> OnUseEffectInstance;
    private List<Effect> OnHitEffectInstance;

    private void InitOnUseEffects()
    {
        OnUseEffectInstance = new List<Effect>();
        for (int i = 0; i < OnUseEffect.Count; i++)
        {
            OnUseEffectInstance.Add( ScriptableObject.Instantiate(OnUseEffect[i]) );
        }
    }

    private void InitOnHitEffects()
    {
        if(OnHitEffectInstance != null)
        {
            OnHitEffectInstance.Clear();
        }

        OnHitEffectInstance = new List<Effect>();
        for (int i = 0; i < OnHitEffect.Count; i++)
        {
            OnHitEffectInstance.Add(ScriptableObject.Instantiate(OnHitEffect[i]));
        }
    }

    public virtual void Init(Unit u)
    {
        ready = true;
        UsedBy = u;
        slave = new GameObject("Slave").AddComponent<CoroutineSlave>();
        slave.transform.parent = UsedBy.transform;
    }

    public virtual void OnUse(AbilityHitDetector AHD = null)
    {
        if (ready)
        {
            InitOnUseEffects();

            for (int i = 0; i < OnUseEffectInstance.Count; i++)
            {
                if(AHD != null)
                {

                    OnUseEffectInstance[i].Trigger(AHD);
                }
                else
                {
                    OnUseEffectInstance[i].Trigger(this);
                }
            }

            if (cooldown > 0)
            {
                slave.StartCoroutine(CoolDownCoroutine());
            }
        }
    }

    public virtual void OnHit(AbilityHitDetector attack, GameObject hit)
    {
        InitOnHitEffects();

        for (int i = 0; i < OnHitEffectInstance.Count; i++)
        {
            if(hit == null)
            {
                OnHitEffectInstance[i].Trigger(attack);
            }
            else
            {
                OnHitEffectInstance[i].Trigger(attack, hit);
            }
        }
    }

    protected IEnumerator CoolDownCoroutine()
    {
        ready = false;
        yield return new WaitForSeconds(cooldown);
        ready = true;
    }
}
