using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ModifiableInt
{
    [NonSerialized] private int baseValue;
    [SerializeField] private int modifiedValue;

    public int BaseValue
    {
        set
        {
            baseValue = value;
            UpdateModifiedValue();
        }
        get => baseValue;
    }

    public int ModifiedValue { set => modifiedValue = value; get => modifiedValue; }

    private event Action<ModifiableInt> OnModifiedValue;

    private List<IModifier> modifiers = new List<IModifier>();

    public ModifiableInt(Action<ModifiableInt> p_method = null)
    {
        ModifiedValue = baseValue;
        RegisterModEvent(p_method);
    }

    public void RegisterModEvent(Action<ModifiableInt> p_method)
    {
        if (p_method != null) OnModifiedValue += p_method;
    }

    public void UnregisterModEvent(Action<ModifiableInt> p_method)
    {
        if (p_method != null) OnModifiedValue -= p_method;
    }

    private void UpdateModifiedValue()
    {
        int t_valueToAdd = 0;

        foreach (IModifier t_modifier in modifiers)
        {
            t_modifier.AddValue(ref t_valueToAdd);
        }

        ModifiedValue = baseValue + t_valueToAdd;

        OnModifiedValue?.Invoke(this);
    }

    public void AddModifier(IModifier p_modifier)
    {
        modifiers.Add(p_modifier);
        UpdateModifiedValue();
    }

    public void RemoveModifier(IModifier p_modifier)
    {
        modifiers.Remove(p_modifier);
        UpdateModifiedValue();
    }
}
