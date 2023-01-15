using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New State", menuName = "Pluggable AI/State")]
public class State : ScriptableObject
{
    public Action[] actions;
    public Transition[] transitions;
    public Color sceneGizmoColor = Color.gray;

    public void DoActions(StateController p_controller)
    {
        for (int i = 0; i < actions.Length; i++) actions[i].Act(p_controller);
    }

    public void OnEnableActions(StateController p_controller)
    {
        for (int i = 0; i < actions.Length; i++) actions[i].OnReadyAction(p_controller);

        for (int i = transitions.Length - 1; i >= 0; i--) transitions[i].decision.OnEnableDecision(p_controller);
    }

    public void CheckTransitions(StateController p_controller)
    {
        for (int i = 0; i < transitions.Length; i++)
        {
            bool t_decision = transitions[i].decision.Decide(p_controller);
            p_controller.TransitionToState(t_decision ? transitions[i].trueState : transitions[i].falseState, transitions[i].decision);

            if (p_controller.curState != this)
            {
                p_controller.curState.OnEnableActions(p_controller);
                break;
            }
        }
    }
}
