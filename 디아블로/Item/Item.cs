using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Item
{
    public int id = -1;
    public string name;

    public ItemBuff[] buffs;

    public Item()
    {
        id = -1;
        name = "";
    }

    public Item(ItemObject p_itemObject)
    {
        name = p_itemObject.name;
        id = p_itemObject.data.id;

        buffs = new ItemBuff[p_itemObject.data.buffs.Length];
        for (int i = 0; i < buffs.Length; ++i)
        {
            buffs[i] = new ItemBuff(p_itemObject.data.buffs[i].Min, p_itemObject.data.buffs[i].Max);
            buffs[i].stat = p_itemObject.data.buffs[i].stat;
        }
    }
}
