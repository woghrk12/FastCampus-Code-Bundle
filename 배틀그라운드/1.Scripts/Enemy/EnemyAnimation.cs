using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimation : MonoBehaviour
{
    [HideInInspector] public Animator anim;

    [HideInInspector] public float curAimingAngleGap;
    [HideInInspector] public Transform muzzleTr;
    [HideInInspector] public float angularSpeed;

    private StateController controller;
    private NavMeshAgent navAgent;
    private bool isPendingAim;

    private Transform hips, spine;
    private Vector3 initialRootRotation;
    private Vector3 initialHipsRotation;
    private Vector3 initialSpineRotation;
    private Quaternion lastRotation;

    private float timeCountAim, timeCountGuard;
    private readonly float turnSpeed = 25f;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<StateController>();
        navAgent = GetComponent<NavMeshAgent>();

        navAgent.updateRotation = false;

        hips = anim.GetBoneTransform(HumanBodyBones.Hips);
        spine = anim.GetBoneTransform(HumanBodyBones.Spine);

        initialRootRotation = (hips.parent == transform) ? Vector3.zero : hips.parent.localEulerAngles;
        initialHipsRotation = hips.localEulerAngles;
        initialSpineRotation = spine.localEulerAngles;

        anim.SetTrigger(FC.AnimatorKey.ChangeWeapon);
        anim.SetInteger(FC.AnimatorKey.Weapon, (int)Enum.Parse(typeof(EWeaponType), controller.classStats.WeaponType));

        foreach (Transform t_childTr in anim.GetBoneTransform(HumanBodyBones.RightHand))
        {
            muzzleTr = t_childTr.Find("Muzzle");
            if (muzzleTr != null) break;
        }

        foreach (Rigidbody t_rigid in GetComponentsInChildren<Rigidbody>()) t_rigid.isKinematic = true;
    }

    private void Update()
    {
        NavAnimSetUp();
    }

    private void LateUpdate()
    {
        if (controller.IsAiming)
        {
            Vector3 t_dir = controller.personalTarget - spine.position;
            if (t_dir.magnitude < 0.01f || t_dir.magnitude > 1000000.0f) return;

            Quaternion t_targetRotation = Quaternion.LookRotation(t_dir);
            t_targetRotation *= Quaternion.Euler(initialRootRotation);
            t_targetRotation *= Quaternion.Euler(initialHipsRotation);
            t_targetRotation *= Quaternion.Euler(initialSpineRotation);

            t_targetRotation *= Quaternion.Euler(FC.VectorHelper.ToVector(controller.classStats.AimOffset));
            Quaternion t_frameRotation = Quaternion.Slerp(lastRotation, t_targetRotation, timeCountAim);

            if (Quaternion.Angle(t_frameRotation, hips.rotation) <= 60.0f)
            {
                spine.rotation = t_frameRotation;
                timeCountAim += Time.deltaTime;
            }
            else
            {
                if (timeCountAim == 0 && Quaternion.Angle(t_frameRotation, hips.rotation) > 70.0f)
                    StartCoroutine(controller.UnstuckAim(2f));

                spine.rotation = lastRotation;
                timeCountAim = 0;
            }

            lastRotation = spine.rotation;
            Vector3 t_target = controller.personalTarget - muzzleTr.position;
            Vector3 t_forward = muzzleTr.forward;
            curAimingAngleGap = Vector3.Angle(t_target, t_forward);

            timeCountGuard = 0;
        }
        else
        {
            lastRotation = spine.rotation;
            spine.rotation *= Quaternion.Slerp(Quaternion.Euler(FC.VectorHelper.ToVector(controller.classStats.AimOffset)), Quaternion.identity, timeCountGuard);
            timeCountGuard += Time.deltaTime;
        }
    }

    private void OnAnimatorMove()
    {
        if (Time.timeScale > 0 && Time.deltaTime > 0)
        {
            navAgent.velocity = anim.deltaPosition / Time.deltaTime;
            if (!controller.IsStrafing) transform.rotation = anim.rootRotation;
        }
    }

    private void SetUp(float p_speed, float p_angle, Vector3 p_strafeDir)
    {
        p_angle *= Mathf.Deg2Rad;
        angularSpeed = p_angle / controller.generalStats.angleResponseTime;

        anim.SetFloat(FC.AnimatorKey.Speed, p_speed, controller.generalStats.speedDampTime, Time.deltaTime);
        anim.SetFloat(FC.AnimatorKey.AngularSpeed, angularSpeed, controller.generalStats.angularSpeedDampTime, Time.deltaTime);

        anim.SetFloat(FC.AnimatorKey.Horizontal, p_strafeDir.x, controller.generalStats.speedDampTime, Time.deltaTime);
        anim.SetFloat(FC.AnimatorKey.Vertical, p_strafeDir.z, controller.generalStats.speedDampTime, Time.deltaTime);
    }

    private void NavAnimSetUp()
    {
        float t_speed = Vector3.Project(navAgent.desiredVelocity, transform.forward).magnitude;
        float t_angle = 0f;

        if (controller.isFocusSight)
        {
            Vector3 t_dest = (controller.personalTarget - transform.position);
            t_dest.y = 0.0f;
            t_angle = Vector3.SignedAngle(transform.forward, t_dest, transform.up);
            if (controller.IsStrafing)
            {
                t_dest = t_dest.normalized;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(t_dest), turnSpeed * Time.deltaTime);
            }
        }
        else
        {
            if (navAgent.desiredVelocity == Vector3.zero) t_angle = 0.0f;
            else t_angle = Vector3.SignedAngle(transform.forward, navAgent.desiredVelocity, transform.up);
        }

        if (!controller.IsStrafing && Mathf.Abs(t_angle) < controller.generalStats.angleDeadZone)
        {
            transform.LookAt(transform.position + navAgent.desiredVelocity);
            t_angle = 0.0f;
            if (isPendingAim && controller.isFocusSight)
            {
                controller.IsAiming = true;
                isPendingAim = false;
            }
        }

        Vector3 t_dir = navAgent.desiredVelocity;
        t_dir.y = 0.0f;
        t_dir = t_dir.normalized;
        t_dir = Quaternion.Inverse(transform.rotation) * t_dir;
        SetUp(t_speed, t_angle, t_dir);
    }

    public void ActivatePendingAim() { isPendingAim = true; }

    public void AbortPendingAim()
    {
        isPendingAim = false;
        controller.IsAiming = false;
    }
    
}
