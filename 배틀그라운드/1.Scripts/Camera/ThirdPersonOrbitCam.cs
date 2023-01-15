using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ThirdPersonOrbitCam : MonoBehaviour
{
    public Transform player;
    public Vector3 pivotOffset = new Vector3(0.0f, 1.0f, 0.0f);
    public Vector3 camOffset = new Vector3(0.4f, 0.5f, -2.0f);

    public float smooth = 10f;
    public float horizontalAimSpeed = 6.0f;
    public float verticalAimSpeed = 6.0f;
    public float maxVerticalAngle = 30.0f;
    public float minVerticalAngle = -60.0f;
    private float angleMouseH = 0.0f;
    private float angleMouseV = 0.0f;

    public float recoilAngleBounce = 5.0f;
    private float recoilMaxVerticleAngle;
    private float recoilMinVerticleAngle;
    private float recoilAngle = 0f;

    private Camera mainCam;
    private Transform cameraTransform;

    private Vector3 toCameraPos;
    private float toCameraDist;

    private Vector3 smoothPivotOffset;
    private Vector3 smoothCamOffset;
    private Vector3 targetPivotOffset;
    private Vector3 targetCamOffset;

    private float defaultFOV;
    private float targetFOV;

    public float AngleMouseH { get => angleMouseH; }

    private void Awake()
    {
        mainCam = Camera.main;
        cameraTransform = mainCam.transform;

        cameraTransform.position = player.position + Quaternion.identity * pivotOffset + Quaternion.identity * camOffset;
        cameraTransform.rotation = Quaternion.identity;

        toCameraPos = cameraTransform.position - player.position;
        toCameraDist = toCameraPos.magnitude - 0.5f;

        smoothPivotOffset = pivotOffset;
        smoothCamOffset = camOffset;
        defaultFOV = mainCam.fieldOfView;
        angleMouseH = player.eulerAngles.y;

        ResetTargetOffset();
        ResetFOV();
        ResetMaxVerticalAngle();
    }

    private void Update()
    {
        angleMouseH += Mathf.Clamp(Input.GetAxis("Mouse X"), -1f, 1f) * horizontalAimSpeed;
        angleMouseV += Mathf.Clamp(Input.GetAxis("Mouse Y"), -1f, 1f) * verticalAimSpeed;

        angleMouseV = Mathf.Clamp(angleMouseV, minVerticalAngle, maxVerticalAngle);
        angleMouseV = Mathf.LerpAngle(angleMouseV, angleMouseV + recoilAngle, 10f * Time.deltaTime);

        Quaternion t_camYRotation = Quaternion.Euler(0.0f, angleMouseH, 0.0f);
        Quaternion t_aimRotation = Quaternion.Euler(-angleMouseV, angleMouseH, 0.0f);
        
        cameraTransform.rotation = t_aimRotation;

        mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, targetFOV, Time.deltaTime);

        Vector3 t_basePos = player.position + t_camYRotation * targetPivotOffset;
        Vector3 t_noCollisionOffset = targetCamOffset;

        for (float t_zOffset = targetCamOffset.z; t_zOffset <= 0f; t_zOffset += 0.5f)
        {
            t_noCollisionOffset.z = t_zOffset;
            if (DoubleCheckViewingPos(t_basePos + t_aimRotation * t_noCollisionOffset, Mathf.Abs(t_zOffset)) || t_zOffset == 0f) break;
        }

        smoothPivotOffset = Vector3.Lerp(smoothPivotOffset, targetPivotOffset, smooth * Time.deltaTime);
        smoothCamOffset = Vector3.Lerp(smoothCamOffset, t_noCollisionOffset, smooth * Time.deltaTime);

        cameraTransform.position = player.position + t_camYRotation * smoothPivotOffset + t_aimRotation * smoothCamOffset;

        if (recoilAngle > 0.0f) recoilAngle -= recoilAngleBounce * Time.deltaTime;
        else if (recoilAngle < 0.0f) recoilAngle += recoilAngleBounce * Time.deltaTime;
    }

    public float GetCurPivotDist(Vector3 p_finalPivotOffset) { return Mathf.Abs((p_finalPivotOffset - smoothPivotOffset).magnitude); }

    public void ResetTargetOffset()
    {
        targetPivotOffset = pivotOffset;
        targetCamOffset = camOffset;
    }

    public void ResetFOV()
    {
        targetFOV = defaultFOV;
    }

    public void ResetMaxVerticalAngle()
    {
        recoilMaxVerticleAngle = maxVerticalAngle;
    }

    public void BounceVertical(float p_degree)
    {
        recoilAngle = p_degree;
    }

    public void SetTargetOffset(Vector3 p_newPivotOffset, Vector3 p_newCamOffset)
    {
        targetPivotOffset = p_newPivotOffset;
        targetCamOffset = p_newCamOffset;
    }

    public void SetFOV(float p_customFOV)
    {
        targetFOV = p_customFOV;
    }

    private bool CheckViewingPos(Vector3 p_checkPos, float p_deltaPlayerHeight)
    {
        Vector3 t_target = player.position + (Vector3.up * p_deltaPlayerHeight);

        if (Physics.SphereCast(p_checkPos, 0.2f, t_target - p_checkPos, out RaycastHit t_hitInfo, toCameraDist))
            if (t_hitInfo.transform != player && !t_hitInfo.transform.GetComponent<Collider>().isTrigger) return false;

        return true;
    }

    private bool CheckInvViewingPos(Vector3 p_checkPos, float p_deltaPlayerHeight, float p_maxDist)
    {
        Vector3 t_origin = player.position + (Vector3.up * p_deltaPlayerHeight);
        if (Physics.SphereCast(t_origin, 0.2f, p_checkPos - t_origin, out RaycastHit t_hitInfo, p_maxDist))
            if (t_hitInfo.transform != player && t_hitInfo.transform != cameraTransform && !t_hitInfo.transform.GetComponent<Collider>().isTrigger)
                return false;

        return true;
    }

    private bool DoubleCheckViewingPos(Vector3 p_checkPos, float p_offset)
    {
        float t_playerFocusHeight = player.GetComponent<CapsuleCollider>().height * 0.75f;
        return CheckViewingPos(p_checkPos, t_playerFocusHeight) && CheckInvViewingPos(p_checkPos, t_playerFocusHeight, p_offset);
    }
}
