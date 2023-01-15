using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ESense { NEAR, PERCEPTION, VIEW, }

[CreateAssetMenu(fileName = "New Focus Decision", menuName = "Pluggable AI/Decisions/Focus")]
public class FocusDecision : Decision
{
    public ESense sense;
    private float radius;

    public bool isInvalidateCoverSpot;

    public override void OnEnableDecision(StateController p_controller)
    {
        switch (sense)
        {
            case ESense.NEAR:
                radius = p_controller.nearRadius;
                break;
            case ESense.PERCEPTION:
                radius = p_controller.perceptionRadius;
                break;
            case ESense.VIEW:
                radius = p_controller.viewRadius;
                break;
            default:
                Debug.LogWarning("Unregistered Enum Type");
                radius = p_controller.nearRadius;
                break;
        }
    }

    public override bool Decide(StateController p_controller)
    {
        return (sense != ESense.NEAR && p_controller.variables.isFeelAlert && !p_controller.BlockedSight())
            || CheckTargetsInRadius(p_controller, radius, HandleTargets);
    }

    private bool HandleTargets(StateController p_controller, bool p_hasTarget, Collider[] p_targetInRadius)
    {
        if (p_hasTarget && !p_controller.BlockedSight())
        {
            if (isInvalidateCoverSpot) p_controller.CoverSpot = Vector3.positiveInfinity;

            p_controller.isTargetInsight = true;
            p_controller.personalTarget = p_controller.aimTarget.position;

            return true;
        }

        return false;
    }
}
