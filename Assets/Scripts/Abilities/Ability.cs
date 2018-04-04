using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/Ability", fileName = "New Ability")]
public class Ability : ScriptableObject {

    public delegate void AbilityDelegate();
    public AbilityDelegate OnAbilityUse;

    [HideInInspector]
    public Unit UsedBy;
    [HideInInspector]
    public CoroutineSlave slave;
    public Sprite AbilityBarImage;
    public float damage;
    public float cooldown;
    public float coolDownRemaining;
    public int maxCharges;

    public int currentCharges;
    [HideInInspector]
    public bool ready;
    public List<Effect> OnUseEffect;
    public List<Effect> OnHitEffect;

    private List<Effect> OnUseEffectInstance;
    private List<Effect> OnHitEffectInstance;

    [HideInInspector]
    public string foeTag;
    [HideInInspector]
    public string foeAttackTag;

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

        foeTag = UsedBy.gameObject.tag == "Player" ? "Enemy" : "Player";
        foeAttackTag = foeTag == "Enemy" ? "EnemyProjectile" : "PlayerProjectile";

        currentCharges = maxCharges;

    }

    public virtual void OnUse(AbilityHitDetector AHD = null)
    {
        if (ready || currentCharges > 0)
        {
            if(maxCharges != 0)
            {
                currentCharges--;
            }

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
                if(OnAbilityUse != null)
                {
                   OnAbilityUse();
                }
                if( CDC == null)
                {
                    CDC = slave.StartCoroutine(CoolDownCoroutine());
                }
                coolDownRemaining += cooldown;
            }
        }
    }

    public virtual void OnHit(AbilityHitDetector attack, GameObject hit)
    {
        InitOnHitEffects();

        if(hit.tag == foeTag)
        {
            if(foeTag == "Enemy")
            {
                hit.gameObject.GetComponent<Enemy>().ChangeHealth(-(int)damage);
            }
            else if(foeTag == "Player")
            {
                hit.gameObject.GetComponent<Player>().ChangeHealth(-(int)damage);
            }
        }

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

    Coroutine CDC;

    protected IEnumerator CoolDownCoroutine()
    {
        ready = false;

        while (coolDownRemaining > 0)
        {
            coolDownRemaining =- 1f;
            yield return new WaitForSeconds(.1f);
        }
        if(maxCharges != 0)
        {
            currentCharges = Mathf.Clamp(currentCharges + 1, 0, maxCharges);
        }
        coolDownRemaining = 0;
        ready = true;
        CDC = null;
    }
}


[System.Serializable]
public class AbilityStruct
{
    public Ability prefab;
    [System.NonSerialized]
    public Ability instance;
}
