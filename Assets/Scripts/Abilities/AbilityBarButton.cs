using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBarButton : MonoBehaviour {

    public UnityEngine.UI.Button button;
    public UnityEngine.UI.Image AbilityImage;
    public UnityEngine.UI.Image CoolDownFade;
    public UnityEngine.UI.Image selected;

    private Ability ability;
    bool cd = false;

    private Coroutine CDR;

    public void Init(Ability a)
    {
        this.ability = a;
        ability.OnAbilityUse += ShowCoolDown;
        if(ability.AbilityBarImage != null)
        {
            AbilityImage.sprite = ability.AbilityBarImage;
            CoolDownFade.sprite = ability.AbilityBarImage;
        }
        CoolDownFade.fillAmount = 0;
    }

    public void ShowCoolDown()
    {
        if(CDR != null)
        {
            StopCoroutine(CDR);
        }
        CDR = StartCoroutine(CoolDown());
    }

    IEnumerator CoolDown()
    {
        CoolDownFade.fillAmount = 1;

        float t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / ability.cooldown;
            CoolDownFade.fillAmount = Mathf.Lerp(1f, 0f, t);
            yield return new WaitForEndOfFrame();
        }
    }
}
