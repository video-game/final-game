using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHud : MonoBehaviour {

    Player player;

    public UnityEngine.UI.Slider healthSlider;
    public TextMeshProUGUI healthText;

    public UnityEngine.UI.Slider koSlider;
    public TextMeshProUGUI koText;

    private bool KOd;

    public void Init(Player p)
    {
        player = p;
        p.OnHealthChange += UpdateHealth;
        p.OnPlayerKO += playerKO;
        p.OnPlayerRevive += playerRevive;

        KOd = false;

        UpdateHealth(p.CurrentHealth, p.MaxHealth);
    }

    private void UpdateHealth(int current, int max)
    {
        UnityEngine.UI.Slider slider = KOd ? koSlider : healthSlider;
        TextMeshProUGUI text = KOd ? koText : healthText;

        text.text = current + " / " + max;
        slider.value = (float)current / max;
    }

    private void playerKO()
    {
        KOd = true;
        healthSlider.gameObject.SetActive(false);
        koSlider.gameObject.SetActive(true);
    }

    private void playerRevive()
    {
        KOd = false;
        healthSlider.gameObject.SetActive(true);
        koSlider.gameObject.SetActive(false);
    }
}
