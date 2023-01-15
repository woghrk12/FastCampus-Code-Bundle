using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class Inventory
{
    public InventorySlot[] slots = new InventorySlot[24];

    public void Clear()
    {
        foreach (InventorySlot t_slot in slots)
        {
            t_slot.RemoveItem();
        }
    }

    public bool IsContain(ItemObject p_itemObject)
    {
        return Array.Find(slots, x => x.item.id == p_itemObject.data.id) != null;
    }

    public bool IsContain(int p_id)
    {
        return slots.FirstOrDefault(x => x.item.id == p_id) != null;
    }
}
