using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimBehaviour : GenericBehaviour
{
    private Transform charTransform;

    public Texture2D crossHair;
    public float aimTurnSmooth = 0.15f;
    public Vector3 aimPivotOffset = new Vector3(0.5f, 1.2f, 0.0f);
    public Vector3 aimCamOffset = new Vector3(0.0f, 0.4f, -0.7f);

    private int aimHash;
    private bool isAim;

    private int cornerHash;
    private bool isPeekCorner;

    private Vector3 initialRootRotation;
    private Vector3 initialHipRotation;
    private Vector3 initialSpineRotation;

    private void Start()
    {
        charTransform = transform;

        aimHash = Animator.StringToHash(FC.AnimatorKey.Aim);
        cornerHash = Animator.StringToHash(FC.AnimatorKey.Corner);

        Transform t_hips = controller.Anim.GetBoneTransform(HumanBodyBones.Hips);
        initialRootRotation = (t_hips.parent == transform) ? Vector3.zero : t_hips.parent.localEulerAngles;
        initialHipRotation = t_hips.localEulerAngles;
        initialSpineRotation = controller.Anim.GetBoneTransform(HumanBodyBones.Spine).localEulerAngles;
    }

    private void Update()
    {
        isPeekCorner = controller.Anim.GetBool(cornerHash);

        if (Input.GetAxisRaw(ButtonName.Aim) != 0 && !isAim) StartCoroutine(ToggleAimOn());
        else if (isAim && Input.GetAxisRaw(ButtonName.Aim) == 0) StartCoroutine(ToggleAimOff());

        canSprint = !isAim;

        if (isAim && Input.GetButtonDown(ButtonName.Shoulder) && !isPeekCorner) 
        {
            aimCamOffset.x = aimCamOffset.x * (-1);
            aimPivotOffset.x = aimPivotOffset.x * (-1);
        }

        controller.Anim.SetBool(aimHash, isAim);
    }

    private void OnGUI()
    {
        if (crossHair != null) 
        {
            float t_dist = controller.CamScript.GetCurPivotDist(aimPivotOffset);

            if (t_dist < 0.05f)
            {
                GUI.DrawTexture(new Rect(Screen.width * 0.5f - (crossHair.width * 0.5f), 
                    Screen.height * 0.5f - (crossHair.height * 0.5f), 
                    crossHair.width, 
                    crossHair.height), crossHair);
            }
        }
    }

    public override void LocalFixedUpdate()
    {
        if (isAim) controller.CamScript.SetTargetOffset(aimPivotOffset, aimCamOffset);
    }

    public override void LocalLateUpdate()
    {
        Aim();
    }

    private void Rotate()
    {
        Vector3 t_forward = controller.camTransform.TransformDirection(Vector3.forward);
        t_forward.y = 0.0f;
        t_forward = t_forward.normalized;

        Quaternion t_targetRotation = Quaternion.Euler(0f, controller.CamScript.AngleMouseH, 0.0f);
        float t_minSpeed = Quaternion.Angle(charTransform.rotation, t_targetRotation) * aimTurnSmooth;

        if (isPeekCorner)
        {
            charTransform.rotation = Quaternion.LookRotation(-controller.LastDirection);
            t_targetRotation *= Quaternion.Euler(initialRootRotation);
            t_targetRotation *= Quaternion.Euler(initialHipRotation);
            t_targetRotation *= Quaternion.Euler(initialSpineRotation);
            controller.Anim.GetBoneTransform(HumanBodyBones.Spine).rotation = t_targetRotation;
        }
        else
        {
            controller.LastDirection = t_forward;
            charTransform.rotation = Quaternion.Slerp(charTransform.rotation, t_targetRotation, t_minSpeed * Time.deltaTime);
        }
    }

    private void Aim()
    {
        Rotate();
    }

    private IEnumerator ToggleAimOn()
    {
        yield return new WaitForSeconds(0.05f);

        if (controller.GetLockStatus(behaviourCode) || controller.CheckIsOverride(this)) yield return false;
        else
        {
            isAim = true;
            int t_signal = 1;
            if (isPeekCorner) t_signal = (int)Mathf.Sign(controller.HAxis);
            aimCamOffset.x = Mathf.Abs(aimCamOffset.x) * t_signal;
            aimPivotOffset.x = Mathf.Abs(aimPivotOffset.x) * t_signal;

            yield return new WaitForSeconds(0.1f);

            controller.Anim.SetFloat(speedHash, 0.0f);
            controller.OverrideBehaviour(this);
        }
    }

    private IEnumerator ToggleAimOff()
    {
        isAim = false;
        
        yield return new WaitForSeconds(0.3f);

        controller.CamScript.ResetTargetOffset();
        controller.CamScript.ResetMaxVerticalAngle();

        yield return new WaitForSeconds(0.1f);

        controller.RevokeOverridingBehaviour(this);
    }
}
