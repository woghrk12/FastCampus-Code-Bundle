using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DynamicInventoryUI : InventoryUI
{
    [SerializeField] protected GameObject slotPrefab;
    [SerializeField] protected Vector2 start;
    [SerializeField] protected Vector2 size;
    [SerializeField] protected Vector2 space;
    [Min(1), SerializeField] protected int numOfColumn = 4;

    protected override void CreateSlotUIs()
    {
        slotUIs = new Dictionary<GameObject, InventorySlot>();
        for (int i = 0; i < inventoryObject.Slots.Length; ++i)
        {
            GameObject t_uiObj = Instantiate(slotPrefab, Vector3.zero, Quaternion.identity, transform);
            t_uiObj.GetComponent<RectTransform>().anchoredPosition = CalculatePosition(i);

            AddEvent(t_uiObj, EventTriggerType.PointerEnter, delegate { OnEnterSlot(t_uiObj); });
            AddEvent(t_uiObj, EventTriggerType.PointerExit, delegate { OnExitSlot(t_uiObj); });
            AddEvent(t_uiObj, EventTriggerType.BeginDrag, delegate { OnStartDrag(t_uiObj); });
            AddEvent(t_uiObj, EventTriggerType.Drag, delegate { OnDrag(t_uiObj); });
            AddEvent(t_uiObj, EventTriggerType.EndDrag, delegate { OnEndDrag(t_uiObj); });
            AddEvent(t_uiObj, EventTriggerType.PointerClick, (t_data) => { OnClick(t_uiObj, (PointerEventData)t_data); });

            inventoryObject.Slots[i].slotUI = t_uiObj;
            slotUIs.Add(t_uiObj, inventoryObject.Slots[i]);
        }
    }

    private Vector2 CalculatePosition(int p_idx)
    {
        var t_floatX = start.x + (space.x + size.x) * (p_idx % numOfColumn);
        var t_floatY = start.y - (space.y + size.y) * (p_idx % numOfColumn);
        return new Vector2(t_floatX, t_floatY);
    }

    protected override void OnLeftClick(InventorySlot p_slot)
    {
        base.OnLeftClick(p_slot);
    }

    protected override void OnRightClick(InventorySlot p_slot)
    {
        
    }
}
