using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBehaviour : GenericBehaviour
{
    public float walkSpeed = 0.15f;
    public float runSpeed = 1.0f;
    public float sprintSpeed = 2.0f;
    public float speedDampTime = 0.1f;
    public float speed, speedSeeker;

    private bool isJump = false;
    public float jumpHeight = 1.5f;
    public float jumpForce = 10f;

    private int jumpHash;
    private int groundedHash;

    private bool isCollided;
    private CapsuleCollider capsuleCollider;

    private Transform charTransform;

    private void Start()
    {
        charTransform = transform;
        capsuleCollider = GetComponent<CapsuleCollider>();

        jumpHash = Animator.StringToHash(FC.AnimatorKey.Jump);
        groundedHash = Animator.StringToHash(FC.AnimatorKey.Grounded);

        controller.Anim.SetBool(groundedHash, true);

        controller.AddBehaviour(this);
        controller.SetDefaultBehaviour(behaviourCode);

        speedSeeker = runSpeed;
    }

    private void Update()
    {
        if (!isJump && Input.GetButtonDown(ButtonName.Jump) && controller.CheckIsCurBehaviour(behaviourCode) && !controller.CheckIsOverride())
        {
            isJump = true;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        isCollided = true;

        if (controller.CheckIsCurBehaviour(BehaviourCode) && collision.GetContact(0).normal.y <= 0.1f)
        {
            float t_velocity = controller.Anim.velocity.magnitude;
            Vector3 t_tangentMove = Vector3.ProjectOnPlane(charTransform.forward, collision.GetContact(0).normal).normalized * t_velocity;
            controller.Rigid.AddForce(t_tangentMove, ForceMode.VelocityChange);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        isCollided = false;
    }

    public override void LocalFixedUpdate()
    {
        Move(controller.HAxis, controller.VAxis);
        Jump();
    }

    private Vector3 Rotate(float p_horizontal, float p_vertical)
    {
        Vector3 t_forward = controller.camTransform.TransformDirection(Vector3.forward);
        
        t_forward.y = 0.0f;
        t_forward = t_forward.normalized;

        Vector3 t_right = new Vector3(t_forward.z, 0.0f, -t_forward.x);
        Vector3 t_targetDir = t_forward * p_vertical + t_right * p_horizontal;

        if (controller.IsMove && t_targetDir != Vector3.zero)
        {
            Quaternion t_newRotation = Quaternion.Slerp(controller.Rigid.rotation, Quaternion.LookRotation(t_targetDir), controller.turnSmooth);
            controller.Rigid.MoveRotation(t_newRotation);
            controller.LastDirection = t_targetDir;
        }

        if (!(Mathf.Abs(p_horizontal) > 0.9f || Mathf.Abs(p_vertical) > 0.9f)) controller.Repositioning();

        return t_targetDir;
    }

    private void RemoveVerticalVelocity()
    {
        Vector3 t_horizontalVelocity = controller.Rigid.velocity;
        t_horizontalVelocity.y = 0.0f;
        controller.Rigid.velocity = t_horizontalVelocity;
    }

    private void Move(float p_horizontal, float p_vertical)
    {
        if (controller.IsGrounded)
            controller.Rigid.useGravity = true;
        else if (!controller.Anim.GetBool(jumpHash) && controller.Rigid.velocity.y > 0)
            RemoveVerticalVelocity();

        Rotate(p_horizontal, p_vertical);
        
        if (controller.IsSprint)
            speed = sprintSpeed;
        else
        {
            Vector2 t_dir = new Vector2(p_horizontal, p_vertical);

            speed = Vector2.ClampMagnitude(t_dir, 1f).magnitude;
            speedSeeker += Input.GetAxis("Mouse ScrollWheel");
            speedSeeker = Mathf.Clamp(speedSeeker, walkSpeed, runSpeed);
            speed *= speedSeeker;
        }

        controller.Anim.SetFloat(speedHash, speed, speedDampTime, Time.deltaTime);
    }

    private void Jump()
    {
        if (isJump && !controller.Anim.GetBool(jumpHash) && controller.IsGrounded)
        {
            controller.LockBehaviour(behaviourCode);
            controller.Anim.SetBool(jumpHash, true);

            if (controller.Anim.GetFloat(speedHash) > 0.1f)
            {
                capsuleCollider.material.dynamicFriction = 0f;
                capsuleCollider.material.staticFriction = 0f;

                RemoveVerticalVelocity();

                float t_velocity = 2f * Mathf.Abs(Physics.gravity.y) * jumpHeight;
                t_velocity = Mathf.Sqrt(t_velocity);
                controller.Rigid.AddForce(Vector3.up * t_velocity, ForceMode.VelocityChange);
            }
        }
        else if (controller.Anim.GetBool(jumpHash))
        {
            if (!controller.IsGrounded && !isCollided && controller.GetLockStatus())
                controller.Rigid.AddForce(charTransform.forward * jumpForce * Physics.gravity.magnitude * speed, ForceMode.Acceleration);
            if (controller.Rigid.velocity.y < 0f && controller.IsGrounded)
            {
                controller.Anim.SetBool(groundedHash, true);
                capsuleCollider.material.dynamicFriction = 0.6f;
                capsuleCollider.material.staticFriction = 0.6f;
                isJump = false;
                controller.Anim.SetBool(jumpHash, false);
                controller.UnlockBehaviour(behaviourCode);
            }
        }
    }
}
