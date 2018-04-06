using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBar : MonoBehaviour {

    private Player player;
    public List<AbilityBarButton> abilities;

    public AbilityBarButton Dash;

    public void Init(Player p)
    {
        player = p;
        player.OnAbilitySelected += Select;
        player.bar = this;
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

    public void AddAbility(Ability ability)
    {
        AbilityBarButton button;
        for (int i = 0; i < abilities.Count; i++)
        {
            if(abilities[i].ability == null)
            {
                button = abilities[i];
                button.Init(ability);
                ability.Init(player);
                break;
            }
        }

    }

}
