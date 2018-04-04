using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBar : MonoBehaviour {

    private Player player;
    public List<AbilityBarButton> abilities;

    public void Init(Player p)
    {
        player = p;

        player.OnNewAbility += UpdateAbilityBar;
        player.OnAbilityUsed += Click;
        player.OnAbilitySelected += Select;
    }

    public void Click(int index)
    {
        abilities[index].Click();
    }

    public void Select(int index)
    {
        player.selectedAbilityIndex = index;
        for (int i = 0; i < abilities.Count; i++)
        {
            if(i == index)
            {
                abilities[i].selected.gameObject.SetActive(true);
            }
            else
            {
                abilities[i].selected.gameObject.SetActive(false);
            }
        }
    }

    public void UpdateAbilityBar(List<RangedAbility> newList)
    {
        Debug.Log(newList.Count);
        for (int i = 0; i < newList.Count; i++)
        {
            if(newList[i] != null)
            {
                abilities[i].enabled = true;
                abilities[i].Init(newList[i]);
            }
            else
            {
                abilities[i].enabled = false;
            }
        }
    }
}
