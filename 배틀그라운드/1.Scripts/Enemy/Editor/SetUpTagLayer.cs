using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SetUpTagLayer : Editor
{
    [MenuItem("GameObject/Enemy AI/Set up Tag and Layers", false, 11)]
    private static void Init()
    {
        GameObject t_obj = Selection.activeGameObject;
        t_obj.tag = "Enemy";
        t_obj.layer = LayerMask.NameToLayer(FC.TagAndLayer.LayerName.Enemy);

        GameObject t_hips = t_obj.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Hips).gameObject;
        if (!t_hips.GetComponent<Collider>()) t_hips = t_hips.transform.GetChild(0).gameObject;
        t_hips.layer = LayerMask.NameToLayer(FC.TagAndLayer.LayerName.Enemy);
        
        t_obj.GetComponentInChildren<SkinnedMeshRenderer>().gameObject.layer = LayerMask.NameToLayer(FC.TagAndLayer.LayerName.IgnoreRayCast);

        foreach (Transform t_childTr in t_obj.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.RightHand))
        {
            Transform t_muzzleTr = t_childTr.Find("Muzzle");
            if (t_muzzleTr != null)
            {
                t_childTr.gameObject.layer = LayerMask.NameToLayer(FC.TagAndLayer.LayerName.IgnoreRayCast);
                foreach (Transform t_partTr in t_childTr) t_partTr.gameObject.layer = t_childTr.gameObject.layer;
            }
        }
    }
}

[InitializeOnLoad]
public class StartUp
{
    static StartUp()
    {
        if (LayerMask.NameToLayer(FC.TagAndLayer.LayerName.Enemy) != 12)
            Debug.LogWarning("Enemy Layer Missing");
    }
}