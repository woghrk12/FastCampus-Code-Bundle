using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class MouseData
{
    public static InventoryUI interfaceMouseIsOver;
    public static GameObject slotMouseIsOver;
    public static GameObject tempItemBeingDragged;
}


[RequireComponent(typeof(EventTrigger))]
public abstract class InventoryUI : MonoBehaviour
{
    public InventoryObject inventoryObject;
    private InventoryObject previousInventoryObject;

    public Dictionary<GameObject, InventorySlot> slotUIs = new Dictionary<GameObject, InventorySlot>();

    private void Awake()
    {
        CreateSlotUIs();
        
        for (int i = 0; i < inventoryObject.Slots.Length; i++)
        {
            inventoryObject.Slots[i].parent = inventoryObject;
            inventoryObject.Slots[i].OnPostUpdate += OnPostUpdate;
        }

        AddEvent(gameObject, EventTriggerType.PointerEnter, delegate { OnEnterInterface(gameObject); });
        AddEvent(gameObject, EventTriggerType.PointerExit, delegate { OnExitInterface(gameObject); });
    }

    protected virtual void Start()
    {
        for (int i = 0; i < inventoryObject.Slots.Length; i++)
        {
            inventoryObject.Slots[i].UpdateSlot(inventoryObject.Slots[i].item, inventoryObject.Slots[i].amount);
        }
    }

    protected abstract void CreateSlotUIs();

    protected void AddEvent(GameObject p_obj, EventTriggerType t_type, UnityAction<BaseEventData> p_action)
    {
        EventTrigger t_trigger = p_obj.GetComponent<EventTrigger>();
        if (!t_trigger)
        {
            Debug.LogWarning("No EventTrigger component found!");
            return;
        }

        EventTrigger.Entry t_entry = new EventTrigger.Entry { eventID = t_type };
        t_entry.callback.AddListener(p_action);
        t_trigger.triggers.Add(t_entry);
    }

    public void OnPostUpdate(InventorySlot p_slot)
    {
        p_slot.slotUI.transform.GetChild(0).GetComponent<Image>().sprite = p_slot.item.id < 0 ? null : p_slot.ItemObject.icon;
        p_slot.slotUI.transform.GetChild(0).GetComponent<Image>().color = p_slot.item.id < 0 ? new Color(1f, 1f, 1f, 0f) : new Color(1f, 1f, 1f, 1f);
        p_slot.slotUI.GetComponentInChildren<TextMesh>().text = p_slot.item.id < 0
            ? string.Empty
            : (p_slot.amount == 1 ? string.Empty : p_slot.amount.ToString("N0"));
    }

    public void OnEnterInterface(GameObject p_obj)
    {
        MouseData.interfaceMouseIsOver = p_obj.GetComponent<InventoryUI>();
    }

    public void OnExitInterface(GameObject p_obj)
    {
        MouseData.interfaceMouseIsOver = null;
    }

    public void OnEnterSlot(GameObject p_obj)
    {
        MouseData.slotMouseIsOver = p_obj;
    }

    public void OnExitSlot(GameObject p_obj)
    {
        MouseData.slotMouseIsOver = null;
    }

    private GameObject CreateDragImage(GameObject p_obj)
    {
        if (slotUIs[p_obj].item.id < 0) return null;
        GameObject t_dragImageObj = new GameObject();

        RectTransform t_rectTransform = t_dragImageObj.AddComponent<RectTransform>();
        t_rectTransform.sizeDelta = new Vector2(50, 50);
        t_dragImageObj.transform.SetParent(transform.parent);

        Image t_image = t_dragImageObj.AddComponent<Image>();
        t_image.sprite = slotUIs[p_obj].ItemObject.icon;
        t_image.raycastTarget = false;

        t_dragImageObj.name = "Drag Image";

        return t_dragImageObj;
    }

    public void OnStartDrag(GameObject p_obj)
    {
        MouseData.tempItemBeingDragged = CreateDragImage(p_obj);
    }

    public void OnDrag(GameObject p_obj)
    {
        if (MouseData.tempItemBeingDragged == null) return;
        MouseData.tempItemBeingDragged.GetComponent<RectTransform>().position = Input.mousePosition;
    }

    public void OnEndDrag(GameObject p_obj)
    {
        Destroy(MouseData.tempItemBeingDragged);

        if (MouseData.interfaceMouseIsOver == null)
        {
            slotUIs[p_obj].RemoveItem();
        }
        else if (MouseData.slotMouseIsOver)
        {
            InventorySlot t_slot = MouseData.interfaceMouseIsOver.slotUIs[MouseData.slotMouseIsOver];
            inventoryObject.SwapItems(slotUIs[p_obj], t_slot);
        }
    }

    public void OnClick(GameObject p_obj, PointerEventData p_data)
    {
        InventorySlot t_slot = slotUIs[p_obj];
        if (t_slot == null) return;
        if (p_data.button == PointerEventData.InputButton.Left) OnLeftClick(t_slot);
        else if (p_data.button == PointerEventData.InputButton.Right) OnRightClick(t_slot);
    }

    protected virtual void OnRightClick(InventorySlot p_slot)
    { 
    
    }

    protected virtual void OnLeftClick(InventorySlot p_slot)
    {

    }
}
