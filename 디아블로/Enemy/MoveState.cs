using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveState : State<EnemyController>
{
    #region Variables

    private Animator animator = null;
    private CharacterController controller = null;
    private NavMeshAgent agent = null;

    private int hashMove = Animator.StringToHash("Walk");
    private int hashMoveSpeed = Animator.StringToHash("MoveSpeed");

    #endregion Variables

    public override void OnInitialized()
    {
        animator = context.GetComponent<Animator>();
        controller = context.GetComponent<CharacterController>();
        agent = context.GetComponent<NavMeshAgent>();
    }

    public override void OnEnter()
    {
        agent?.SetDestination(context.Target.position);
        animator?.SetBool(hashMove, true);
    }

    public override void OnUpdate(float p_deltaTime)
    {
        Transform t_enemy = context.SearchEnemy();

        if (!t_enemy
            || agent.remainingDistance <= agent.stoppingDistance
            || context.IsAvailableAttack) stateMachine.ChangeState<IdleState>();

        agent.SetDestination(context.Target.position);
        controller.Move(agent.velocity * p_deltaTime);
        animator.SetFloat(hashMoveSpeed, agent.velocity.magnitude / agent.speed, 1f, p_deltaTime);
    }

    public override void OnExit()
    {
        animator?.SetBool(hashMove, false);
        animator?.SetFloat(hashMoveSpeed, 0f);
        agent?.ResetPath();
    }
}
