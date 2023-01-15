using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCBattleUI : MonoBehaviour
{
    private Slider hpSlider;
    [SerializeField] private GameObject damageTextPrefab;

    public float MinimumValue
    {
        get => hpSlider.minValue;
        set { hpSlider.minValue = value; }
    }
    public float MaximumValue
    {
        get => hpSlider.maxValue;
        set { hpSlider.maxValue = value; }
    }
    public float CurValue
    {
        get => hpSlider.value;
        set { hpSlider.value = value; }
    }

    private void Awake()
    {
        hpSlider = gameObject.GetComponentInChildren<Slider>();
    }

    private void OnEnable()
    {
        GetComponent<Canvas>().enabled = true;
    }

    private void OnDisable()
    {
        GetComponent<Canvas>().enabled = false;
    }

    public void CreateDamageText(int p_damage)
    {
        if (damageTextPrefab != null)
        {
            GameObject t_damageTextGO = Instantiate(damageTextPrefab, transform);
            DamageText t_damageText = t_damageTextGO.GetComponent<DamageText>();
            if (t_damageText == null) Destroy(t_damageTextGO);
            t_damageText.Damage = p_damage;
        }
    }
}
