using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFacing : MonoBehaviour
{
    private Camera referCamera;
    public bool isReverseFace = false;

    public enum EAxis { UP, DOWN ,LEFT, RIGHT, FOWARD, BACK };

    public EAxis axis = EAxis.UP;
    public Vector3 GetAxis(EAxis p_refAxis)
    {
        switch (p_refAxis)
        {
            case EAxis.DOWN:
                return Vector3.down;
            case EAxis.LEFT:
                return Vector3.left;
            case EAxis.RIGHT:
                return Vector3.right;
            case EAxis.FOWARD:
                return Vector3.forward;
            case EAxis.BACK:
                return Vector3.back;
            default:
                return Vector3.up;
        }

    }

    private void Awake()
    {
        if (!referCamera) referCamera = Camera.main;
    }

    private void LateUpdate()
    {
        Vector3 t_targetPos = transform.position + referCamera.transform.rotation * (isReverseFace ? Vector3.forward : Vector3.back);
        Vector3 t_targetRot = referCamera.transform.rotation * GetAxis(axis);

        transform.LookAt(t_targetPos, t_targetRot);
    }
}
