using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentCombiner
{
    private readonly Dictionary<int, Transform> rootBoneDictionary = new Dictionary<int, Transform>();
    private readonly Transform rootTransform;

    public EquipmentCombiner(GameObject p_rootObj)
    {
        rootTransform = p_rootObj.transform;
        TraverseHierarchy(rootTransform);
    }

    public Transform AddLimb(GameObject p_itemObj, List<string> p_boneNames)
    {
        Transform t_limb = ProcessBoneObject(p_itemObj.GetComponentInChildren<SkinnedMeshRenderer>(), p_boneNames);
        t_limb.SetParent(rootTransform);

        return t_limb;
    }

    private Transform ProcessBoneObject(SkinnedMeshRenderer p_renderer, List<string> p_boneNames)
    {
        GameObject t_itemObj = new GameObject();

        SkinnedMeshRenderer t_renderer = t_itemObj.AddComponent<SkinnedMeshRenderer>();

        Transform[] t_boneTransforms = new Transform[p_boneNames.Count];
        for (int i = 0; i < p_boneNames.Count; ++i)
            t_boneTransforms[i] = rootBoneDictionary[p_boneNames[i].GetHashCode()];

        t_renderer.bones = t_boneTransforms;
        t_renderer.sharedMesh = p_renderer.sharedMesh;
        t_renderer.materials = p_renderer.sharedMaterials;

        return t_itemObj.transform;
    }

    public Transform[] AddMesh(GameObject p_itemObj)
    {
        return ProcessMeshobject(p_itemObj.GetComponentsInChildren<MeshRenderer>());
    }

    private Transform[] ProcessMeshobject(MeshRenderer[] p_renderers)
    {
        List<Transform> t_itemTransforms = new List<Transform>();

        foreach (MeshRenderer t_renderer in p_renderers)
        {
            if (t_renderer.transform.parent != null)
            {
                Transform t_parent = rootBoneDictionary[t_renderer.transform.parent.name.GetHashCode()];

                GameObject t_itemObj = GameObject.Instantiate(t_renderer.gameObject, t_parent);
                t_itemTransforms.Add(t_itemObj.transform);
            }
        }

        return t_itemTransforms.ToArray();
    }

    private void TraverseHierarchy(Transform p_root)
    {
        foreach (Transform t_child in p_root)
        {
            rootBoneDictionary.Add(t_child.name.GetHashCode(), t_child);
            TraverseHierarchy(t_child);
        }
    }
}
