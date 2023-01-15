using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : HealthBase
{
    public float health = 100f;
    public float criticalHealth = 30f;
    public Transform healthHUD;
    public SoundList deathSound;
    public SoundList hitSound;
    public GameObject hurtEffect;
    public float decayFactor = 0.8f;

    private float totalHealth;
    private RectTransform healthBar, placeHolderBar;
    private Text healthLabel;
    private float originalBarScale;
    private bool isCritical;

    private BlinkHUD criticalHUD;
    private HurtHUD hurtHUD;

    public bool IsFullHealth { get { return Mathf.Abs(health - totalHealth) < float.Epsilon; } }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        totalHealth = health;

        healthBar = healthHUD.Find("HealthBar/Bar").GetComponent<RectTransform>();
        placeHolderBar = healthHUD.Find("HealthBar/Placeholder").GetComponent<RectTransform>();
        healthLabel = healthHUD.Find("HealthBar/Label").GetComponent<Text>();

        originalBarScale = healthBar.sizeDelta.x;
        healthLabel.text = ((int)health).ToString();

        criticalHUD = healthHUD.Find("Bloodframe").GetComponent<BlinkHUD>();
        hurtHUD = gameObject.AddComponent<HurtHUD>();
        hurtHUD.SetUp(healthHUD, hurtEffect, decayFactor, transform);
    }

    private void Update()
    {
        if (placeHolderBar.sizeDelta.x > healthBar.sizeDelta.x)
            placeHolderBar.sizeDelta = Vector2.Lerp(placeHolderBar.sizeDelta, healthBar.sizeDelta, 2f * Time.deltaTime);
    }

    private void UpdateHealthBar()
    {
        healthLabel.text = ((int)health).ToString();
        healthBar.sizeDelta = new Vector2((health / totalHealth) * originalBarScale, healthBar.sizeDelta.y);
    }

    private void Die()
    {
        IsDead = true;
        gameObject.layer = FC.TagAndLayer.GetLayerByName(FC.TagAndLayer.LayerName.Default);
        gameObject.tag = FC.TagAndLayer.TagName.Untagged;
        
        healthHUD.gameObject.SetActive(false);
        healthHUD.parent.Find("WeaponHUD").gameObject.SetActive(false);
        
        anim.SetBool(FC.AnimatorKey.Aim, false);
        anim.SetBool(FC.AnimatorKey.Cover, false);
        anim.SetFloat(FC.AnimatorKey.Speed, 0);

        foreach (GenericBehaviour t_behaviour in GetComponentsInChildren<GenericBehaviour>())
            t_behaviour.enabled = false;

        SoundManager.Instance.PlayOneShotEffect((int)deathSound, transform.position, 5f);
    }

    public override void TakeDamage(Vector3 p_pos, Vector3 p_dir, float p_damage, Collider p_bodyPart = null, GameObject p_hitEffect = null)
    {
        health -= p_damage;
        
        UpdateHealthBar();

        if (hurtEffect && healthHUD) hurtHUD.DrawHurtUI(p_hitEffect.transform, p_hitEffect.GetHashCode());

        if (health <= 0) Die();
        else if (health <= criticalHealth && !isCritical)
        {
            isCritical = true;
            criticalHUD.StartBlink();
        }

        SoundManager.Instance.PlayOneShotEffect((int)hitSound, p_pos, 1f);
    }
}
