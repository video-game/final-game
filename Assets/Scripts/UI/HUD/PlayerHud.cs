﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHud : MonoBehaviour {

    Player player;

    public UnityEngine.UI.Slider healthSlider;
    public TextMeshProUGUI healthText;

    public UnityEngine.UI.Slider koSlider;
    public TextMeshProUGUI koText;

    public UnityEngine.UI.Slider experienceSlider;
    public TextMeshProUGUI experienceText;
    public TextMeshProUGUI levelText;

    private bool KOd;

    int level = 1;
    public void Init(Player p)
    {
        player = p;
        player.OnHealthChange += UpdateHealth;
        player.OnPlayerKO += PlayerKO;
        player.OnPlayerRevive += PlayerRevive;
        player.OnExperienceGained += UpdateExperience;
        player.OnPlayerLvlUp += LevelUp;

        KOd = false;

        levelText.text = level.ToString();

        UpdateExperience(player.Experience, player.nextLevel);
        UpdateHealth(player.CurrentHealth, player.MaxHealth);
    }

    private void UpdateHealth(int current, int max)
    {
        UnityEngine.UI.Slider slider = KOd ? koSlider : healthSlider;
        TextMeshProUGUI text = KOd ? koText : healthText;

        text.text = current + " / " + max;
        slider.value = (float)current / max;
    }

    private void UpdateExperience(int current, int max)
    {
        experienceText.text = current + " / " + max;
        experienceSlider.value = (float)current / max;
    }

    private void PlayerKO()
    {
        KOd = true;
        healthSlider.gameObject.SetActive(false);
        koSlider.gameObject.SetActive(true);
    }

    private void PlayerRevive()
    {
        KOd = false;
        healthSlider.gameObject.SetActive(true);
        koSlider.gameObject.SetActive(false);
    }

    private void LevelUp()
    {
        level++;
        levelText.text = level.ToString();
    }
}
