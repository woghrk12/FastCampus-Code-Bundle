using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatUI : MonoBehaviour
{
    public InventoryObject equipment;
    public StatObject playerStats;

    public Text[] attributeText;

    private void OnEnable()
    {
        playerStats.OnChangeStats += OnChangeStats;
        if (equipment != null && playerStats != null)
        {
            foreach (InventorySlot t_slot in equipment.Slots)
            {
                t_slot.OnPreUpdate += OnRemoveItem;
                t_slot.OnPostUpdate += OnEquipItem;
            }
        }

        UpdateAttributeText();
    }

    private void OnDisable()
    {
        playerStats.OnChangeStats -= OnChangeStats;
        if (equipment != null && playerStats != null)
        {
            foreach (InventorySlot t_slot in equipment.Slots)
            {
                t_slot.OnPreUpdate -= OnRemoveItem;
                t_slot.OnPostUpdate -= OnEquipItem;
            }
        }
    }

    private void OnChangeStats(StatObject p_statObject)
    {
        UpdateAttributeText();
    }

    private void OnEquipItem(InventorySlot p_slot)
    {
        if (p_slot.ItemObject == null) return;

        foreach (ItemBuff t_buff in p_slot.item.buffs)
        {
            foreach (Attribute t_attribute in playerStats.attributes)
            {
                if (t_attribute.type == t_buff.stat)
                    t_attribute.value.AddModifier(t_buff);
            }
        }
    }

    private void OnRemoveItem(InventorySlot p_slot)
    {
        if (p_slot.ItemObject == null) return;

        foreach (ItemBuff t_buff in p_slot.item.buffs)
        {
            foreach (Attribute t_attribute in playerStats.attributes)
            {
                if (t_attribute.type == t_buff.stat)
                    t_attribute.value.RemoveModifier(t_buff);
            }
        }
    }

    private void UpdateAttributeText()
    {
        attributeText[0].text = playerStats.GetModifiedValue(ECharacterAttribute.Agility).ToString("N0");
        attributeText[0].text = playerStats.GetModifiedValue(ECharacterAttribute.Intellect).ToString("N0");
        attributeText[0].text = playerStats.GetModifiedValue(ECharacterAttribute.Stamina).ToString("N0");
        attributeText[0].text = playerStats.GetModifiedValue(ECharacterAttribute.Strength).ToString("N0");
    }
}
