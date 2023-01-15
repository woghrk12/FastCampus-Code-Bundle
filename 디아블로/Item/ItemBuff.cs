using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ECharacterAttribute 
{
    Health,
    Mana,
    Agility,
    Intellect,
    Stamina,
    Strength
}

[Serializable]
public class ItemBuff : IModifier
{
    public ECharacterAttribute stat;
    public int value;
    [SerializeField] private int minValue;
    [SerializeField] private int maxValue;

    public int Min => minValue;
    public int Max => maxValue;

    public ItemBuff(int p_min, int p_max)
    {
        minValue = p_min;
        maxValue = p_max;

        GenerateValue();
    }

    private void GenerateValue()
    {
        value = UnityEngine.Random.Range(minValue, maxValue);
    }

    public void AddValue(ref int p_value)
    {
        p_value += value;
    }
}
