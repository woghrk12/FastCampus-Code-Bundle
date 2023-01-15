using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventorySlot
{
    public EItemType[] allowedTypes = new EItemType[0];

    [NonSerialized] public InventoryObject parent;
    [NonSerialized] public GameObject slotUI;

    [NonSerialized] public Action<InventorySlot> OnPreUpdate;
    [NonSerialized] public Action<InventorySlot> OnPostUpdate;

    public Item item;
    public int amount;

    public ItemObject ItemObject { get { return item.id >= 0 ? parent.database.itemObjects[item.id] : null; } }

    public InventorySlot() => UpdateSlot(new Item(), 0);
    public InventorySlot(Item p_item, int p_amount) => UpdateSlot(p_item, p_amount);

    public void AddItem(Item p_item, int p_amount) => UpdateSlot(p_item, p_amount);
    public void RemoveItem() => UpdateSlot(new Item(), 0);
    public void AddAmount(int p_value) => UpdateSlot(item, amount += p_value);

    public void UpdateSlot(Item p_item, int p_amount)
    {
        if (p_amount <= 0) { p_item = new Item(); }
        OnPreUpdate?.Invoke(this);
        item = p_item;
        amount = p_amount;
        OnPostUpdate?.Invoke(this);
    }

    public bool CanPlaceInSlot(ItemObject p_itemObject)
    {
        if (allowedTypes.Length <= 0 || p_itemObject == null || ItemObject.data.id < 0) return true;

        foreach (EItemType t_type in allowedTypes)
        {
            if (p_itemObject.type == t_type) return true;
        }

        return false;
    }
}
