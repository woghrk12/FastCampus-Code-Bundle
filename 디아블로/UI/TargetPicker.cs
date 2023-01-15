using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPicker : MonoBehaviour
{
    public float surfaceOffset = 1.5f;
    public Transform target = null;

    void Update()
    {
        if (target) transform.position = target.position + Vector3.up * surfaceOffset;
    }

    public void SetPosition(RaycastHit p_hitInfo)
    {
        target = null;
        transform.position = p_hitInfo.point + p_hitInfo.normal * surfaceOffset;
    }
}
