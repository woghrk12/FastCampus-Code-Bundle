using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericBehaviour : MonoBehaviour
{
    protected BehaviourController controller;

    protected int speedHash;
    protected bool canSprint; 
    public bool CanSprint { get => canSprint; }

    protected int behaviourCode;
    public int BehaviourCode { get => behaviourCode; }    

    private void Awake()
    {
        controller = GetComponent<BehaviourController>();
        speedHash = Animator.StringToHash(FC.AnimatorKey.Speed);
        canSprint = true;

        behaviourCode = GetType().GetHashCode();
    }

    public virtual void LocalLateUpdate() { }
    public virtual void LocalFixedUpdate() { }
    public virtual void OnOverride() { }
}

public class BehaviourController : MonoBehaviour
{
    private List<GenericBehaviour> behaviours;
    private List<GenericBehaviour> overrideBehaviours;
    private int curBehaviour;
    private int defaultBehaviour;
    private int lockedBehaviour;

    private Transform charTransform;
    public Transform camTransform;
    private ThirdPersonOrbitCam camScript;
    private Animator anim;
    private Rigidbody rigid;

    private float hAxis;
    private float vAxis;
    public float turnSmooth = 0.06f;
    private bool isChangedFOV;
    public float sprintFOV = 100f;
    private Vector3 lastDirection;
    private bool isSprint;
    private int hHash;
    private int vHash;
    private int groundedHash;
    private Vector3 colExtents;

    public float HAxis { get => hAxis; }
    public float VAxis { get => vAxis; }
    public ThirdPersonOrbitCam CamScript { get => camScript; }
    public Rigidbody Rigid { get => rigid; }
    public Animator Anim { get => anim; }
    public int DefaultBehaviour { get => defaultBehaviour; }
    public Vector3 LastDirection { set => lastDirection = value; get => lastDirection; }

    public bool IsMove { get { return Mathf.Abs(hAxis) > Mathf.Epsilon || Mathf.Abs(vAxis) > Mathf.Epsilon; } }
    public bool IsHorizontalMoving { get { return Mathf.Abs(hAxis) > Mathf.Epsilon; } }
    public bool CanSprint
    {
        get
        {
            foreach (GenericBehaviour t_behaviour in behaviours)
                if (!t_behaviour.CanSprint) return false;

            foreach (GenericBehaviour t_behaviour in overrideBehaviours)
                if (!t_behaviour.CanSprint) return false;

            return true;
        }
    }
    public bool IsSprint { get { return isSprint && IsMove && CanSprint; } }
    public bool IsGrounded
    {
        get
        {
            Ray t_ray = new Ray(charTransform.position + Vector3.up * 2 * colExtents.x, Vector3.down);
            return Physics.SphereCast(t_ray, colExtents.x, colExtents.x + 0.2f);
        }
    }

    private void Awake()
    {
        behaviours = new List<GenericBehaviour>();
        overrideBehaviours = new List<GenericBehaviour>();

        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();

        charTransform = transform;
        camScript = camTransform.GetComponent<ThirdPersonOrbitCam>();

        hHash = Animator.StringToHash(FC.AnimatorKey.Horizontal);
        vHash = Animator.StringToHash(FC.AnimatorKey.Vertical);
        groundedHash = Animator.StringToHash(FC.AnimatorKey.Grounded);
        colExtents = GetComponent<Collider>().bounds.extents;
    }

    private void Update()
    {
        hAxis = Input.GetAxis("Horizontal");
        vAxis = Input.GetAxis("Vertical");

        anim.SetFloat(hHash, hAxis, 0.1f, Time.deltaTime);
        anim.SetFloat(vHash, vAxis, 0.1f, Time.deltaTime);

        isSprint = Input.GetButton(ButtonName.Sprint);
        if (IsSprint)
        {
            isChangedFOV = true;
            camScript.SetFOV(sprintFOV);
        }
        else if (isChangedFOV)
        {
            camScript.ResetFOV();
            isChangedFOV = false;
        }

        anim.SetBool(groundedHash, IsGrounded);
    }

    private void FixedUpdate()
    {
        bool t_isAnyBehaviourActive = false;

        if (lockedBehaviour > 0 || overrideBehaviours.Count == 0)
        {
            foreach (GenericBehaviour t_behaviour in behaviours)
                if (t_behaviour.isActiveAndEnabled && curBehaviour == t_behaviour.BehaviourCode)
                {
                    t_isAnyBehaviourActive = true;
                    t_behaviour.LocalFixedUpdate();
                }
        }
        else
        {
            foreach (GenericBehaviour t_behaviour in overrideBehaviours)
                t_behaviour.LocalFixedUpdate();
        }

        if (!t_isAnyBehaviourActive && overrideBehaviours.Count == 0)
        {
            rigid.useGravity = true;
            Repositioning();
        }
    }

    private void LateUpdate()
    {
        if (lockedBehaviour > 0 || overrideBehaviours.Count == 0)
        {
            foreach (GenericBehaviour t_behaviour in behaviours)
                if (t_behaviour.isActiveAndEnabled && curBehaviour == t_behaviour.BehaviourCode)
                    t_behaviour.LocalLateUpdate();
        }
        else
        {
            foreach (GenericBehaviour t_behaviour in overrideBehaviours)
                t_behaviour.LocalLateUpdate();
        }
    }

    public void Repositioning()
    {
        if (lastDirection != Vector3.zero)
        {
            lastDirection.y = 0f;

            Quaternion t_targetRotation = Quaternion.LookRotation(lastDirection);
            Quaternion t_newRotation = Quaternion.Slerp(rigid.rotation, t_targetRotation, turnSmooth);

            rigid.MoveRotation(t_newRotation);
        }
    }

    public void AddBehaviour(GenericBehaviour p_behaviour) => behaviours.Add(p_behaviour);

    public void SetDefaultBehaviour(int p_behaviourCode)
    {
        defaultBehaviour = p_behaviourCode;
        curBehaviour = p_behaviourCode;
    }

    public void RegisterBehaviour(int p_behaviourCode)
    {
        if (curBehaviour != defaultBehaviour) return;

        curBehaviour = p_behaviourCode;
    }

    public void UnregisterBehaviour(int p_behaviourCode)
    {
        if (curBehaviour != p_behaviourCode) return;

        curBehaviour = defaultBehaviour;
    }

    public bool OverrideBehaviour(GenericBehaviour p_behaviour)
    {
        if (overrideBehaviours.Contains(p_behaviour)) return false;

        if (overrideBehaviours.Count == 0)
        {
            foreach (GenericBehaviour t_behaviour in behaviours)
            {
                if (t_behaviour.isActiveAndEnabled && curBehaviour == t_behaviour.BehaviourCode)
                {
                    t_behaviour.OnOverride();
                    break;
                }
            }
        }

        overrideBehaviours.Add(p_behaviour);
        return true;
    }

    public bool RevokeOverridingBehaviour(GenericBehaviour p_behaviour)
    {
        if (!overrideBehaviours.Contains(p_behaviour)) return false;

        overrideBehaviours.Remove(p_behaviour);
        return true;
    }

    public bool CheckIsOverride(GenericBehaviour p_behaviour = null)
    {
        if (p_behaviour == null) return overrideBehaviours.Count > 0;

        return overrideBehaviours.Contains(p_behaviour);
    }

    public bool CheckIsCurBehaviour(int p_behaviourCode)
    {
        return curBehaviour == p_behaviourCode;
    }

    public bool GetLockStatus(int p_behaviourCode = 0)
    {
        return (lockedBehaviour != 0 && lockedBehaviour != p_behaviourCode);
    }

    public void LockBehaviour(int p_behaviourCode)
    {
        if (lockedBehaviour != 0) return;

        lockedBehaviour = p_behaviourCode;
    }

    public void UnlockBehaviour(int p_behaviourCode)
    {
        if (lockedBehaviour != p_behaviourCode) return;

        lockedBehaviour = 0;
    }
}
