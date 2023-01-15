using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualCollision : MonoBehaviour
{
    public Vector3 boxSize = new Vector3(3, 2, 2);

    public Collider[] CheckOverlapBox(LayerMask p_layerMask)
    {
        return Physics.OverlapBox(transform.position, boxSize * 0.5f, transform.rotation, p_layerMask);
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
    }
}
