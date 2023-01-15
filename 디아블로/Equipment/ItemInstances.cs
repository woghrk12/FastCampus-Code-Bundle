using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInstances
{
    public List<Transform> itemTransforms = new List<Transform>();

    public void OnDestroy()
    {
        foreach (Transform t_item in itemTransforms)
        {
            GameObject.Destroy(t_item.gameObject);
        }
    }
}
