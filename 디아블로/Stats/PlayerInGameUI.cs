using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInGameUI : MonoBehaviour
{
    public StatObject playerStats;

    public Text levelText;
    public Image healthSlider;
    public Image manaSlider;

    private void Start()
    {
        levelText.text = playerStats.level.ToString("N0");
        healthSlider.fillAmount = playerStats.HealthPercentage;
        manaSlider.fillAmount = playerStats.ManaPercentage;
    }

    private void OnEnable()
    {
        playerStats.OnChangeStats += OnChangeStats;
    }

    private void OnDisable()
    {
        playerStats.OnChangeStats -= OnChangeStats;
    }

    private void OnChangeStats(StatObject p_statObject)
    {
        levelText.text = playerStats.level.ToString("N0");
        healthSlider.fillAmount = playerStats.HealthPercentage;
        manaSlider.fillAmount = playerStats.ManaPercentage;
    }
}
