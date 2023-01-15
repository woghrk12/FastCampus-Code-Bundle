using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownCamera : MonoBehaviour
{
    #region Variables

    public float height = 5f;
    public float distance = 10f;
    public float angle = 45f;
    public float lookAtHeight = 2f;
    public float smoothSpeed = 0.5f;
    
    private Vector3 refVelocity = Vector3.zero;

    [SerializeField] private Transform target = null;

    public Transform Target { get { return target; } }

    #endregion Variables

    private void LateUpdate()
    {
        HandleCamera();
    }

    #region Helper Methods

    public void HandleCamera()
    {
        if (!target) return;

        Vector3 t_worldPos = (Vector3.forward * -distance) + (Vector3.up * height);
        Vector3 t_rotatedVector = Quaternion.AngleAxis(angle, Vector3.up) * t_worldPos;

        Vector3 t_targetPos = target.position;
        t_targetPos.y += lookAtHeight;

        Vector3 t_finalPos = t_targetPos + t_rotatedVector;

        transform.position = Vector3.SmoothDamp(transform.position, t_finalPos, ref refVelocity, smoothSpeed);
        transform.LookAt(t_targetPos);
    }

    #endregion Helper Methods
}
