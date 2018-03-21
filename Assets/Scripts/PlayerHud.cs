using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHud : MonoBehaviour {

    Player player;

    public UnityEngine.UI.Slider healthSlider;
    public TextMeshProUGUI healthText;

    public void Init(Player p)
    {
        player = p;
        p.OnHealthChange += UpdateHealth;
    }

    private void UpdateHealth(int current, int max)
    {
        healthText.text = current + " / " + max;
        healthSlider.value = (float)current / max;
    }

}
