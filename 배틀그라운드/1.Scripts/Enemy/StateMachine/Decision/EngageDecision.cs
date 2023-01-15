using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Engage Decision", menuName = "Pluggable AI/Decisions/Engage")]
public class EngageDecision : Decision
{
    [Header("Extra Decision")]
    public LookDecision isViewing;
    public FocusDecision targetNear;

    public override bool Decide(StateController p_controller)
    {
        if (isViewing.Decide(p_controller) || targetNear.Decide(p_controller)) p_controller.variables.blindEngageTimer = 0;
        else if (p_controller.variables.blindEngageTimer >= p_controller.blindEngageTime)
        {
            p_controller.variables.blindEngageTimer = 0;
            return false;
        }

        return true;
    }
}
