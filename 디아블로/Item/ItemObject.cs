using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EItemType : int
{ 
    HELMET = 0,
    CHEST = 1,
    PANTS = 2, 
    BOOTS = 3,
    PAULDRONS = 4,
    GLOVES = 5,
    LEFTWEAPON = 6,
    RIGHTWEAPON = 7,
    FOOD,
    DEFAULT
};

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Items/Item")]
public class ItemObject : ScriptableObject
{
    public EItemType type;
    public bool stackable;

    public Sprite icon;
    public GameObject modelPrefab;

    public Item data = new Item();

    public List<string> boneNames = new List<string>();

    [TextArea(15, 20)] public string description;

    private void OnValidate()
    {
        boneNames.Clear();

        if (modelPrefab == null || modelPrefab.GetComponentInChildren<SkinnedMeshRenderer>() == null) return;

        SkinnedMeshRenderer t_renderer = modelPrefab.GetComponentInChildren<SkinnedMeshRenderer>();
        Transform[] t_bones = t_renderer.bones;

        foreach (Transform t_transform in t_bones)
        {
            boneNames.Add(t_transform.name);
        }
    }

    public Item CreateItem()
    {
        Item t_newItem = new Item(this);
        return t_newItem;
    }
}
