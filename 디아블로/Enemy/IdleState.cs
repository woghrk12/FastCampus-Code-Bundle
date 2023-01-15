using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State<EnemyController>
{
    private Animator animator;
    private CharacterController controller;

    protected int hashMove = Animator.StringToHash("Walk");
    protected int hashMoveSpeed = Animator.StringToHash("MoveSpeed");

    private bool isPatrol = false;
    private float minIdleTime = 0.0f;
    private float maxIdleTime = 3.0f;
    private float idleTime = 0.0f;

    public IdleState(bool p_isPatrol)
    {
        isPatrol = p_isPatrol;
    }

    public override void OnInitialized()
    {
        animator = context.GetComponent<Animator>();
        controller = context.GetComponent<CharacterController>();
    }

    public override void OnEnter()
    {
        animator?.SetBool(hashMove, false);
        animator?.SetFloat(hashMoveSpeed, 0f);
        controller?.Move(Vector3.zero);

        if (isPatrol)
        {
            idleTime = Random.Range(minIdleTime, maxIdleTime);
        }
    }

    public override void OnUpdate(float p_deltaTime)
    {
        Transform t_enemy = context.SearchEnemy();
        if (t_enemy)
        {
            if (context.IsAvailableAttack) stateMachine.ChangeState<AttackState>();
            else stateMachine.ChangeState<MoveState>();
        }
        else if (isPatrol && stateMachine.ElapsedTimeInState > idleTime)
        {
            stateMachine.ChangeState<PatrolState>();
        }
    }
}
