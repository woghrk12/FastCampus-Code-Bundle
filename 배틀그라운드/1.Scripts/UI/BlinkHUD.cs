using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class BlinkHUD : MonoBehaviour
{
    public float blinkFadeStep = 1.0f;

    private Image hud;
    private Color originalColor, noAlphaColor;
    private float fadeTimer;
    private float colorGradeTimer;
    private int fadeFactor = 1;
    private int colorGradeFactor = 1;
    private bool isBlink, isLastTime;
    private ColorGrading colorGradingLayer;
    private Bloom bloomLayer;
    private float dyingSaturation;
    private float dyingBrightness;

    private void Start()
    {
        hud = GetComponent<Image>();
        hud.enabled = true;
        originalColor = noAlphaColor = hud.color;
        noAlphaColor.a = 0f;
        gameObject.SetActive(false);
        dyingSaturation = -30f;
        dyingBrightness = -10f;

        PostProcessVolume t_volume = Camera.main.gameObject.GetComponent<PostProcessVolume>();
        t_volume.profile.TryGetSettings(out colorGradingLayer);
        t_volume.profile.TryGetSettings(out bloomLayer);
    }

    private void Update()
    {
        if (isBlink)
        {
            colorGradingLayer.saturation.value = Mathf.Lerp(0, dyingSaturation, colorGradeTimer / blinkFadeStep);
            colorGradingLayer.brightness.value = Mathf.Lerp(0, dyingBrightness, colorGradeTimer / blinkFadeStep);
            bloomLayer.intensity.value = Mathf.Lerp(0, 7f, colorGradeTimer / blinkFadeStep);

            colorGradeTimer += colorGradeFactor * Time.deltaTime;
            
            hud.color = Color.Lerp(noAlphaColor, originalColor, fadeTimer / blinkFadeStep);
            fadeTimer += fadeFactor * Time.deltaTime;

            if (fadeTimer >= blinkFadeStep || fadeTimer <= 0f)
            {
                fadeFactor = -fadeFactor;
                colorGradeFactor = 0;
                if (fadeTimer <= 0f) fadeTimer = 0f;
                else if (fadeTimer >= blinkFadeStep && isLastTime)
                {
                    isBlink = isLastTime = false;
                    gameObject.SetActive(false);
                }
            }
        }
    }

    public void StartBlink()
    {
        gameObject.SetActive(true);
        isBlink = true;
        isLastTime = false;
        colorGradeFactor = 1;
    }

    public void ApplyRespawnFilter()
    {
        colorGradingLayer.saturation.value = -40f;
        colorGradingLayer.brightness.value = 0f;
        bloomLayer.intensity.value = 0;
    }
}
