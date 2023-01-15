using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Hear Decision", menuName = "Pluggable AI/Decisions/Hear")]
public class HearDecision : Decision
{
    private Vector3 lastPos, curPos;

    public override void OnEnableDecision(StateController p_controller)
    {
        lastPos = curPos = Vector3.positiveInfinity;
    }

    private bool HandleTargets(StateController p_controller, bool p_hasTarget, Collider[] p_targetInRadius)
    {
        if (p_hasTarget)
        {
            curPos = p_targetInRadius[0].transform.position;
            if (!Equals(lastPos, Vector3.positiveInfinity))
            {
                if (!Equals(lastPos, curPos))
                {
                    p_controller.personalTarget = curPos;
                    return true;
                }
            }
            lastPos = curPos;
        }

        return false;
    }

    public override bool Decide(StateController p_controller)
    {
        if (p_controller.variables.isHearAlert)
        {
            p_controller.variables.isHearAlert = false;
            return true;
        }

        return CheckTargetsInRadius(p_controller, p_controller.perceptionRadius, HandleTargets);
    }
}
