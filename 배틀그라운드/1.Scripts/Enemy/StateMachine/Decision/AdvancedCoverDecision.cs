using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AdvancedCover Decision", menuName = "Pluggable AI/Decisions/AdvancedCover")]
public class AdvancedCoverDecision : Decision
{
    public int waitRounds = 1;

    [Header("Extra Decision")]
    public FocusDecision targetNear;

    public override void OnEnableDecision(StateController p_controller)
    {
        p_controller.variables.waitRounds += 1;

        p_controller.variables.isAdvanceCoverDecision = Random.Range(0f, 1f) < p_controller.classStats.ChangeCoverChance * 0.01f;
    }

    public override bool Decide(StateController p_controller)
    {
        if (p_controller.variables.waitRounds <= waitRounds) return false;

        p_controller.variables.waitRounds = 0;
        return p_controller.variables.isAdvanceCoverDecision && !targetNear.Decide(p_controller);
    }
}
