using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Has attribute data such as effect prefab, path, and type
/// </summary>
public class EffectClip 
{
    public int clipId = 0;

    public EEffectType effectType = EEffectType.None;
    public GameObject effectPrefab = null;
    public string effectName = string.Empty;
    public string effectPath = string.Empty;
    
    public string EffectFullPath { get { return effectPath + effectName; } }

    public EffectClip() { }

    public void PreLoad()
    {
        if (EffectFullPath == string.Empty) return;
        if (effectPrefab != null) return;

        effectPrefab = ResourceManager.Load(EffectFullPath) as GameObject;
    }

    public void ReleaseClip()
    {
        if (effectPrefab == null) return;

        effectPrefab = null;
    }

    public GameObject Instantiate(Vector3 p_position)
    {
        if (effectPrefab == null) PreLoad();
        return effectPrefab != null ? GameObject.Instantiate(effectPrefab, p_position, Quaternion.identity) : null;
    }
}
