using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, IAttackable, IDamagable
{
    #region Variables

    private Animator anim = null;

    private FieldOfView fov = null;
    protected StateMachine<EnemyController> stateMachine;
    [SerializeField] private float attackRange;
    [SerializeField] private LayerMask targetMask;

    public Transform[] waypoints;
    [HideInInspector] public Transform targetWayPoint = null;
    private int wayPointIdx = 0;

    [SerializeField] private bool isPatrol = false;

    public Transform projectileTransform = null;
    [SerializeField] private List<AttackBehaviour> attackBehaviours = null;

    public Transform hitTransform = null;
    public int maxHealth;
    public int CurHealth { private set; get; }

    private int hitTriggerHash = Animator.StringToHash("Hit");

    [SerializeField] private NPCBattleUI battleUI;

    #endregion Variables

    #region Unity Methods

    private void Awake()
    {
        anim = GetComponent<Animator>();
        fov = GetComponent<FieldOfView>();
        stateMachine = new StateMachine<EnemyController>(this, new PatrolState());
        StateMachine.AddState(new IdleState(isPatrol));
        stateMachine.AddState(new MoveState());
        stateMachine.AddState(new AttackState());

        CurHealth = maxHealth;
    }

    private void Start()
    {
        InitAttackBehaviours();
        if (battleUI)
        {
            battleUI.MinimumValue = 0.0f;
            battleUI.MaximumValue = maxHealth;
            battleUI.CurValue = CurHealth;
        }
    }

    private void Update()
    {
        CheckAttackBehaviour();
        stateMachine.Update(Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    #endregion Unity Methods

    #region Helper Methods

    private void InitAttackBehaviours()
    {
        foreach (AttackBehaviour t_behaviour in attackBehaviours)
        {
            if (CurrentAttackBehaviour == null)
                CurrentAttackBehaviour = t_behaviour;
            t_behaviour.targetMask = targetMask;
        }
    }

    private void CheckAttackBehaviour()
    {
        if (CurrentAttackBehaviour == null || !CurrentAttackBehaviour.IsAvailable)
        {
            CurrentAttackBehaviour = null; 
            foreach (AttackBehaviour t_behaviour in attackBehaviours)
            {
                if (t_behaviour.IsAvailable)
                {
                    if ((CurrentAttackBehaviour == null) ||
                        (CurrentAttackBehaviour.priority < t_behaviour.priority))
                        CurrentAttackBehaviour = t_behaviour;
                }
            }
        }
    }

    public StateMachine<EnemyController> StateMachine { get { return stateMachine; } }

    public Transform Target => fov?.NearestTarget;

    public bool IsAvailableAttack
    {
        get 
        {
            if (!Target) return false;
            float t_dist = Vector3.Distance(transform.position, Target.position);
            return t_dist <= attackRange;
        }
    }
    
    public Transform SearchEnemy()
    {
        return Target;
    }

    public Transform FindNextWayPoint()
    {
        if (waypoints.Length <= 0) return null;

        targetWayPoint = waypoints[wayPointIdx];
        wayPointIdx = (wayPointIdx + 1) % waypoints.Length;

        return targetWayPoint;
    }

    #endregion Helper Methods

    #region IAttackable Interfaces

    public AttackBehaviour CurrentAttackBehaviour { set; get; }

   
    public void OnExecuteAttack(int p_attackIdx)
    {
        if (CurrentAttackBehaviour != null && Target != null)
        {
            CurrentAttackBehaviour.ExecuteAttack(Target.gameObject, projectileTransform);
        }
    }



    #endregion IAttackable Interfaces

    #region IDamagable Interfaces

    public bool IsAlive => CurHealth > 0;
    public void TakeDamage(int p_damage, GameObject p_hitEffectPrefab)
    {
        if (!IsAlive) return;

        CurHealth -= p_damage;
        if (battleUI)
        {
            battleUI.CurValue = CurHealth;
            battleUI.CreateDamageText(p_damage);
        }
        if (p_hitEffectPrefab) Instantiate(p_hitEffectPrefab, hitTransform);
        if (IsAlive) anim?.SetTrigger(hitTriggerHash);
        else
        {
            if (battleUI != null) battleUI.gameObject.SetActive(false);
            stateMachine.ChangeState<DeadState>();
            QuestManager.Instance.ProcessQuest(EQuestType.DESTROYENEMY, 0);
        }
    }

    #endregion IDamagable Interfaces
}
