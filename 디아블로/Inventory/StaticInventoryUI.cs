using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StaticInventoryUI : InventoryUI
{
    public GameObject[] staticSlots = null;

    protected override void CreateSlotUIs()
    {
        slotUIs = new Dictionary<GameObject, InventorySlot>();
        for (int i = 0; i < inventoryObject.Slots.Length; i++)
        {
            GameObject t_uiObj = staticSlots[i];

            AddEvent(t_uiObj, EventTriggerType.PointerEnter, delegate { OnEnterSlot(t_uiObj); });
            AddEvent(t_uiObj, EventTriggerType.PointerExit, delegate { OnExitSlot(t_uiObj); });
            AddEvent(t_uiObj, EventTriggerType.BeginDrag, delegate { OnStartDrag(t_uiObj); });
            AddEvent(t_uiObj, EventTriggerType.Drag, delegate { OnDrag(t_uiObj); });
            AddEvent(t_uiObj, EventTriggerType.EndDrag, delegate { OnEndDrag(t_uiObj); });

            inventoryObject.Slots[i].slotUI = t_uiObj;
            slotUIs.Add(t_uiObj, inventoryObject.Slots[i]);
        }
    }
}
