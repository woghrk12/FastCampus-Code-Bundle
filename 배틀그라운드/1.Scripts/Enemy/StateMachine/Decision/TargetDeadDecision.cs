using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New TargetDead Decision", menuName = "Pluggable AI/Decisions/TargetDead")]
public class TargetDeadDecision : Decision
{
    public override bool Decide(StateController p_controller)
    {
        try
        {
            return p_controller.aimTarget.root.GetComponent<HealthBase>().IsDead;
        }
        catch (UnassignedReferenceException)
        {
            Debug.LogError("Need component related HealthBase " + p_controller.name, p_controller.gameObject);
        }

        return false;
    }
}
