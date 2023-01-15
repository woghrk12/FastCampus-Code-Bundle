using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State<EnemyController>
{
    #region Variables

    private Animator animator = null;
    private AttackStateController attackStateController;
    private IAttackable attackable;

    private int hashAttack = Animator.StringToHash("Attack");
    private int hashAttackIdx = Animator.StringToHash("AttackIdx");

    #endregion Variables

    public override void OnInitialized()
    {
        animator = context.GetComponent<Animator>();
        attackStateController = context.GetComponent<AttackStateController>();
        attackable = context.GetComponent<IAttackable>();
    }

    public override void OnEnter()
    {
        if (attackable is null || attackable.CurrentAttackBehaviour is null)
        {
            stateMachine.ChangeState<IdleState>();
            return;
        }

        attackStateController.enterAttackStateHandler += OnEnterAttackState;
        attackStateController.exitAttackStateHandler += OnExitAttackState;

        animator?.SetInteger(hashAttackIdx, attackable.CurrentAttackBehaviour.animIdx);
        animator?.SetTrigger(hashAttack);
    }

    public override void OnUpdate(float p_deltaTime)
    {
        
    }

    public void OnEnterAttackState()
    { 
    
    }

    public void OnExitAttackState()
    {
        stateMachine.ChangeState<IdleState>();
    }
}
