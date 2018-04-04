using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBarButton : MonoBehaviour {

    public UnityEngine.UI.Button button;
    public UnityEngine.UI.Image AbilityImage;
    public UnityEngine.UI.Image CoolDownFade;
    public UnityEngine.UI.Image selected;

    private RangedAbility ability;
    bool cd = false;

    public void Init(RangedAbility ability)
    {
        this.ability = ability;
        ability.ready = true;
        if(ability.AbilityBarImage != null)
        {
            AbilityImage.sprite = ability.AbilityBarImage;
            CoolDownFade.sprite = ability.AbilityBarImage;
        }
    }

    public void Click()
    {
        if (!cd)
        {
            StartCoroutine(CoolDown(Mathf.Max(ability.coolDownRemaining, ability.cooldown)));
        }

    }

    IEnumerator CoolDown(float cooldown)
    {
        cd = true;
        CoolDownFade.fillAmount = 1;

        var currentPos = transform.position;
        float t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / cooldown;
            CoolDownFade.fillAmount = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }
        cd = false;
    }
}
