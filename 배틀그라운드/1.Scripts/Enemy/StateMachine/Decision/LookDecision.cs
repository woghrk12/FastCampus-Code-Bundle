using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Look Decision", menuName = "Pluggable AI/Decisions/Look")]
public class LookDecision : Decision
{
    private bool HandleTargets(StateController p_controller, bool p_hasTarget, Collider[] p_targetInRadius)
    {
        if (p_hasTarget)
        {
            Vector3 t_target = p_targetInRadius[0].transform.position;
            Vector3 t_dirToTarget = t_target - p_controller.transform.position;
            bool t_isFOVCondition = (Vector3.Angle(p_controller.transform.forward, t_dirToTarget) < p_controller.viewAngle * 0.5f);
            if (t_isFOVCondition && !p_controller.BlockedSight())
            {
                p_controller.isTargetInsight = true;
                p_controller.personalTarget = p_controller.aimTarget.position;
                return true;
            }
        }

        return false;
    }

    public override bool Decide(StateController p_controller)
    {
        p_controller.isTargetInsight = false;
        return CheckTargetsInRadius(p_controller, p_controller.viewRadius, HandleTargets);
    }
}
