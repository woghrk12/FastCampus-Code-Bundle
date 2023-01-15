using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Class that wraps Resources.Load, later changed to Asset Bundle
/// </summary>
public class ResourceManager
{
    public static UnityEngine.Object Load(string p_path)
    {
        return Resources.Load(p_path);
    }

    public static GameObject LoadAndInstantiate(string p_path)
    {
        UnityEngine.Object t_source = Load(p_path);

        if (t_source == null) return null;

        return GameObject.Instantiate(t_source) as GameObject;
    }
}
