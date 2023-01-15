using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EInterfaceType
{ 
    INVENTORY,
    EQUIPMENT,
    QUICKSLOT,
    BOX
}

[CreateAssetMenu(fileName = "New Inventroy", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    public ItemDatabase database;
    public EInterfaceType type;

    [SerializeField] private Inventory container = new Inventory();

    public Action<ItemObject> OnUseItem;

    public InventorySlot[] Slots => container.slots;
    public int EmptySlotCount
    {
        get
        {
            int t_cnt = 0;
            foreach (InventorySlot t_slot in Slots)
                if (t_slot.item.id < 0) t_cnt++;

            return t_cnt;
        }
    }

    public bool AddItem(Item p_item, int p_amount)
    {
        if (database.itemObjects[p_item.id].stackable)
        {
            InventorySlot t_slot = FindItemInInventory(p_item);
            if (t_slot != null)
            {
                t_slot.AddAmount(p_amount);
                QuestManager.Instance.ProcessQuest(EQuestType.ACQUIREITEM, p_item.id);
                return true;
            }
        }

        if (EmptySlotCount <= 0) return false;

        GetEmptySlot().AddItem(p_item, p_amount);
        QuestManager.Instance.ProcessQuest(EQuestType.ACQUIREITEM, p_item.id);
        return true;
    }

    public InventorySlot FindItemInInventory(Item p_item)
    {
        return Slots.FirstOrDefault(x => x.item.id == p_item.id);
    }

    public InventorySlot GetEmptySlot()
    {
        return Slots.FirstOrDefault(x => x.item.id < 0);
    }

    public bool IsContainItem(ItemObject p_itemObject)
    {
        return Slots.FirstOrDefault(x => x.item.id == p_itemObject.data.id) != null;
    }

    public void SwapItems(InventorySlot p_slotA, InventorySlot p_slotB)
    {
        if (p_slotA == p_slotB) return;
        if (!p_slotB.CanPlaceInSlot(p_slotA.ItemObject) || !p_slotA.CanPlaceInSlot(p_slotB.ItemObject)) return;

        InventorySlot t_slot = new InventorySlot(p_slotB.item, p_slotB.amount);
        p_slotB.UpdateSlot(p_slotA.item, p_slotA.amount);
        p_slotA.UpdateSlot(t_slot.item, t_slot.amount);
    }

    public void UseItem(InventorySlot p_slot)
    {
        if (p_slot.ItemObject == null || p_slot.item.id < 0 || p_slot.amount <= 0) return;
        ItemObject t_itemObject = p_slot.ItemObject;
        p_slot.UpdateSlot(p_slot.item, p_slot.amount - 1);
        
        OnUseItem.Invoke(t_itemObject);
    }
}
