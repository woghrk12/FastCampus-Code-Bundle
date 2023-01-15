using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stats", menuName = "Stats System/New Character Stats New")]
public class StatObject : ScriptableObject
{
    public Attribute[] attributes;
    public int level;
    public int exp;
    public int Health { set; get; }
    public int Mana { set; get; }

    public float HealthPercentage
    {
        get
        {
            int t_health = Health;
            int t_maxHealth = Health;

            foreach (Attribute t_attribute in attributes)
            {
                if (t_attribute.type == ECharacterAttribute.Health)
                {
                    t_maxHealth = t_attribute.value.ModifiedValue;
                    break;
                }
            }

            return (t_maxHealth > 0 ? ((float)t_health / (float)t_maxHealth) : 0f);
        }
    }

    public float ManaPercentage
    {
        get
        {
            int t_mana = Mana;
            int t_maxMana = Mana;

            foreach (Attribute t_attribute in attributes)
            {
                if (t_attribute.type == ECharacterAttribute.Mana)
                {
                    t_maxMana = t_attribute.value.ModifiedValue;
                    break;
                }
            }

            return (t_maxMana > 0 ? ((float)t_mana / (float)t_maxMana) : 0f);
        }
    }

    public Action<StatObject> OnChangeStats;

    // Scriptable Object can't use Awake() and Start()

    [NonSerialized] private bool isInit = false;
    public void OnEnable()
    {
        InitializeAttribute();
    }

    public void InitializeAttribute()
    {
        if (isInit) return;

        isInit = true;

        foreach (Attribute t_attribute in attributes)
            t_attribute.value = new ModifiableInt(OnModifiedValue);

        level = 1;
        exp = 0;

        SetBaseValue(ECharacterAttribute.Agility, 100);
        SetBaseValue(ECharacterAttribute.Intellect, 100);
        SetBaseValue(ECharacterAttribute.Stamina, 100);
        SetBaseValue(ECharacterAttribute.Strength, 100);

        Health = GetModifiedValue(ECharacterAttribute.Health);
        Mana = GetModifiedValue(ECharacterAttribute.Mana);
    }

    private void OnModifiedValue(ModifiableInt p_value)
    {
        OnChangeStats?.Invoke(this);
    }

    public void SetBaseValue(ECharacterAttribute p_type, int p_value)
    {
        foreach (Attribute t_type in attributes)
            if (t_type.type == p_type)
                t_type.value.BaseValue = p_value;
    }

    public int GetBaseValue(ECharacterAttribute p_type)
    {
        foreach (Attribute t_type in attributes)
            if (t_type.type == p_type)
                return t_type.value.BaseValue;
        
        return -1;
    }

    public int GetModifiedValue(ECharacterAttribute p_type)
    {
        foreach (Attribute t_type in attributes)
            if (t_type.type == p_type)
                return t_type.value.ModifiedValue;

        return -1;
    }

    public int AddHealth(int p_value)
    {
        Health += p_value;
        OnChangeStats?.Invoke(this);
        return Health;
    }

    public int AddMana(int p_value)
    {
        Mana += p_value;
        OnChangeStats?.Invoke(this);
        return Mana;
    }
}
