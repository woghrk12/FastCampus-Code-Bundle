using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleRandomStateMachineBehaviour : StateMachineBehaviour
{
    #region Variables

    [SerializeField] private int numOfStates = 3;
    [SerializeField] private float minNormTime = 0f;
    [SerializeField] private float maxNormTime = 5f;

    [SerializeField] private float randomNormalTime = 0f;

    private readonly int hashRandomIdle = Animator.StringToHash("RandomIdle");

    #endregion Variables

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        randomNormalTime = Random.Range(minNormTime, maxNormTime);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.IsInTransition(0) && animator.GetCurrentAnimatorStateInfo(0).fullPathHash == stateInfo.fullPathHash)
            animator.SetInteger(hashRandomIdle, -1);

        if (stateInfo.normalizedTime > randomNormalTime && !animator.IsInTransition(0))
            animator.SetInteger(hashRandomIdle, Random.Range(0, numOfStates));
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
