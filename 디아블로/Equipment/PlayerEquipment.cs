using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    public InventoryObject equipment;

    private EquipmentCombiner combiner;

    private ItemInstances[] itemInstances = new ItemInstances[8];

    public ItemObject[] defaultItemObjects = new ItemObject[8];

    private void Awake()
    {
        combiner = new EquipmentCombiner(gameObject);

        for (int i = 0; i < equipment.Slots.Length; ++i)
        {
            equipment.Slots[i].OnPreUpdate += OnRemoveItem;
            equipment.Slots[i].OnPostUpdate += OnEquipItem;
        }
    }

    private void Start()
    {
        foreach (InventorySlot t_slot in equipment.Slots)
            OnEquipItem(t_slot);
    }

    private void OnEquipItem(InventorySlot p_slot)
    {
        ItemObject t_itemObject = p_slot.ItemObject;
        if (t_itemObject == null)
        {
            EquipDefaultItemBy(p_slot.allowedTypes[0]);
            return;
        }

        int t_idx = (int)p_slot.allowedTypes[0];

        switch (p_slot.allowedTypes[0])
        {
            // Skinned Mesh
            case EItemType.HELMET:
            case EItemType.CHEST:
            case EItemType.PANTS:
            case EItemType.BOOTS:
            case EItemType.GLOVES:
                itemInstances[t_idx] = EquipSkinnedItem(t_itemObject);
                break;

            // Static Mesh
            case EItemType.PAULDRONS:
            case EItemType.LEFTWEAPON:
            case EItemType.RIGHTWEAPON:
                itemInstances[t_idx] = EquipStaticItem(t_itemObject);
                break;
        }
    }

    private ItemInstances EquipSkinnedItem(ItemObject p_itemObject)
    {
        if (p_itemObject == null) return null;

        Transform t_itemTransform = combiner.AddLimb(p_itemObject.modelPrefab, p_itemObject.boneNames);
        
        if (t_itemTransform != null)
        {
            ItemInstances t_instance = new ItemInstances();
            t_instance.itemTransforms.Add(t_itemTransform);
            return t_instance;
        }

        return null;
    }

    private ItemInstances EquipStaticItem(ItemObject p_itemObject)
    {
        if (p_itemObject == null) return null;

        Transform[] t_itemTransforms = combiner.AddMesh(p_itemObject.modelPrefab);
        
        if (t_itemTransforms.Length > 0)
        {
            ItemInstances t_instance = new ItemInstances();
            t_instance.itemTransforms.AddRange(t_itemTransforms.ToList<Transform>());
            return t_instance;
        }

        return null;
    }

    private void OnRemoveItem(InventorySlot p_slot)
    {
        ItemObject t_itemObject = p_slot.ItemObject;
        if (t_itemObject == null)
        {
            RemoveItemBy(p_slot.allowedTypes[0]);
            return;
        }

        if (p_slot.ItemObject.modelPrefab != null)
        {
            RemoveItemBy(p_slot.allowedTypes[0]);
        }
    }

    private void RemoveItemBy(EItemType p_type)
    {
        int t_idx = (int)p_type;
        if (itemInstances[t_idx] != null)
        {
            itemInstances[t_idx].OnDestroy();
            itemInstances[t_idx] = null;
        }
    }

    private void EquipDefaultItemBy(EItemType p_type)
    {
        int t_idx = (int)p_type;

        ItemObject t_itemObject = defaultItemObjects[t_idx];

        switch (p_type)
        {
            // Skinned Mesh
            case EItemType.HELMET:
            case EItemType.CHEST:
            case EItemType.PANTS:
            case EItemType.BOOTS:
            case EItemType.GLOVES:
                itemInstances[t_idx] = EquipSkinnedItem(t_itemObject);
                break;

            // Static Mesh
            case EItemType.PAULDRONS:
            case EItemType.LEFTWEAPON:
            case EItemType.RIGHTWEAPON:
                itemInstances[t_idx] = EquipStaticItem(t_itemObject);
                break;
        }
    }

    private void OnDestroy()
    {
        foreach (ItemInstances t_item in itemInstances)
            t_item.OnDestroy();
    }
}
