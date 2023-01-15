using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolState : State<EnemyController>
{
    private Animator animator;
    private CharacterController controller;
    private NavMeshAgent agent;

    protected int hashMove = Animator.StringToHash("Walk");
    protected int hashMoveSpeed = Animator.StringToHash("MoveSpeed");

    public override void OnInitialized()
    {
        animator = context.GetComponent<Animator>();
        controller = context.GetComponent<CharacterController>();
        agent = context.GetComponent<NavMeshAgent>();
    }

    public override void OnEnter()
    {
        if (context.targetWayPoint is null) context.FindNextWayPoint();
        
        if (context.targetWayPoint)
        {
            agent?.SetDestination(context.targetWayPoint.position);
            animator?.SetBool(hashMove, true);
        }
    }

    public override void OnUpdate(float p_deltaTime)
    {
        Transform t_enemy = context.SearchEnemy();
        if (!t_enemy)
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                Transform t_nextDest = context.FindNextWayPoint();
                if (t_nextDest) agent.SetDestination(t_nextDest.position);
                stateMachine.ChangeState<IdleState>();
            }
            else
            {
                controller.Move(agent.velocity * p_deltaTime);
                animator.SetFloat(hashMoveSpeed, agent.velocity.magnitude / agent.speed, .1f, p_deltaTime);
            }
        }
        else
        {
            if (context.IsAvailableAttack) stateMachine.ChangeState<AttackState>();
            else stateMachine.ChangeState<MoveState>();
        }
    }

    public override void OnExit()
    {
        animator?.SetBool(hashMove, false);
        agent?.ResetPath();
    }
}
