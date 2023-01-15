using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : SingletonMonobehaviour<EffectManager>
{
    private Transform effectRoot = null;

    private void Start()
    {
        if (effectRoot == null)
        {
            effectRoot = new GameObject("EffectRoot").transform;
            effectRoot.SetParent(transform);
        }
    }

    public GameObject EffectOneShot(int p_idx, Vector3 p_position)
    {
        GameObject t_effect = DataManager.EffectData.GetClip(p_idx).Instantiate(p_position);
        t_effect.SetActive(true);
        return t_effect;
    }
}
