using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateController : MonoBehaviour
{
    public GeneralStats generalStats;
    public ClassStats statData;
    public string classID;

    public State curState;
    public State remainState;

    public Transform aimTarget;
    public List<Transform> patrolWayPoints;
    [HideInInspector] public int wayPointIdx;

    public int bullets;
    public int maxBurst = 7;

    [Range(0, 50)] public float viewRadius;
    [Range(0, 360)] public float viewAngle;
    [Range(0, 25)] public float perceptionRadius;

    [HideInInspector] public float nearRadius;
    [HideInInspector] public NavMeshAgent nav;

    [HideInInspector] public float blindEngageTime = 30f;
    [HideInInspector] public bool isTargetInsight;
    [HideInInspector] public bool isFocusSight;
    [HideInInspector] public bool isReloading;
    [HideInInspector] public bool hadClearShot;
    [HideInInspector] public bool haveClearShot;

    [HideInInspector] public int coverHash = -1;

    [HideInInspector] public EnemyVariables variables;
    [HideInInspector] public EnemyAnimation enemyAnim;
    [HideInInspector] public CoverLookUp coverLookUp;

    [HideInInspector] public Vector3 personalTarget = Vector3.zero;

    private int magBullets;
    private bool isAIActive;
    private static Dictionary<int, Vector3> coverSpot;
    private bool isStrafing;
    private bool isAiming;
    private bool isCheckedOnLoop;
    private bool isBlockedSight;

    public ClassStats.Param classStats
    {
        get
        {
            foreach (ClassStats.Sheet t_sheet in statData.sheets)
            {
                foreach (ClassStats.Param t_param in t_sheet.list)
                {
                    if (t_param.ID.Equals(classID)) return t_param;
                }
            }
            return null;
        }
    }

    public bool IsStrafing 
    {
        set
        {
            enemyAnim.anim.SetBool(FC.AnimatorKey.Strafe, value);
            isStrafing = value;
        }
        get => isStrafing;
    }

    public bool IsAiming
    {
        set 
        {
            if (isAiming != value)
            {
                enemyAnim.anim.SetBool(FC.AnimatorKey.Aim, value);
                isAiming = value;
            }
        }
        get => isAiming;
    }

    public Vector3 CoverSpot { set { coverSpot[GetHashCode()] = value; } get { return coverSpot[GetHashCode()]; } }

    private void Awake()
    {
        if (coverSpot == null) coverSpot = new Dictionary<int, Vector3>();
        
        coverSpot[GetHashCode()] = Vector3.positiveInfinity;

        nav = GetComponent<NavMeshAgent>();
        enemyAnim = gameObject.AddComponent<EnemyAnimation>();

        isAIActive = true;
        magBullets = bullets;
        variables.shotsInRounds = maxBurst;

        nearRadius = perceptionRadius * 0.5f;

        GameObject t_gameController = GameObject.FindGameObjectWithTag(FC.TagAndLayer.TagName.GameController);
        coverLookUp = t_gameController.GetComponent<CoverLookUp>();
        if (coverLookUp == null)
        {
            coverLookUp = t_gameController.AddComponent<CoverLookUp>();
            coverLookUp.SetUp(generalStats.coverMask);
        }

        Debug.Assert(aimTarget.root.GetComponent<HealthBase>(), "Need HealthBase component");
    }

    private void Start()
    {
        curState.OnEnableActions(this);
    }

    private void Update()
    {
        isCheckedOnLoop = false;

        if (!isAIActive) return;

        curState.DoActions(this);
        curState.CheckTransitions(this);
    }

    private void OnDestroy()
    {
        coverSpot.Remove(GetHashCode());
    }

    private void OnDrawGizmos()
    {
        if (curState != null)
        {
            Gizmos.color = curState.sceneGizmoColor;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 2.5f, 2f);
        }
    }

    public void TransitionToState(State p_nextState, Decision p_decision)
    {
        if (p_nextState == remainState) return;

        curState = p_nextState;
    }

    public IEnumerator UnstuckAim(float p_delay)
    {
        yield return new WaitForSeconds(p_delay * 0.5f);
        IsAiming = false;
        yield return new WaitForSeconds(p_delay * 0.5f);
        IsAiming = true;
    }

    public void EndReloadWeapon()
    {
        isReloading = false;
        bullets = magBullets;
    }

    public void AlertCallback(Vector3 p_target)
    {
        if (!aimTarget.root.GetComponent<HealthBase>().IsDead)
        {
            variables.isHearAlert = true;
            personalTarget = p_target;
        }
    }

    public bool IsNearOtherSpot(Vector3 p_spot, float p_margin = 1f)
    {
        foreach (KeyValuePair<int, Vector3> t_usedSpot in coverSpot)
        {
            if (t_usedSpot.Key != gameObject.GetHashCode() && Vector3.Distance(p_spot, t_usedSpot.Value) <= p_margin) return true;
        }

        return false;
    }

    public bool BlockedSight()
    {
        if (!isCheckedOnLoop)
        {
            isCheckedOnLoop = true;
            
            Vector3 t_target = default;
            try
            {
                t_target = aimTarget.position;
            }
            catch (UnassignedReferenceException)
            {
                Debug.LogError("Assign target for aim : " + transform.name);
            }

            Vector3 t_castOrigin = transform.position + Vector3.up * generalStats.aboveCoverHeight;
            Vector3 t_dirToTarget = t_target - t_castOrigin;

            isBlockedSight = Physics.Raycast(t_castOrigin, t_dirToTarget, out RaycastHit t_hitInfo, t_dirToTarget.magnitude, generalStats.coverMask | generalStats.obstacleMask);
        }

        return isBlockedSight;
    }
}
