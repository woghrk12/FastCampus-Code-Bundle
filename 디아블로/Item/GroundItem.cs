using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroundItem : MonoBehaviour
{
    public ItemObject itemObject;
    public int amount;

    private void OnValidate()
    {
#if UNITY_EDITOR
        GetComponent<SpriteRenderer>().sprite = itemObject?.icon;
        GetComponentInChildren<TextMesh>().text = itemObject ? itemObject.name + amount.ToString() : null;
#endif
    }
}
