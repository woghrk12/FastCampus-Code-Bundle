using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New TakeCover Decision", menuName = "Pluggable AI/Decisions/TakeCover")]
public class TakeCoverDecision : Decision
{
    public override bool Decide(StateController p_controller)
    {
        if (p_controller.variables.curShots < p_controller.variables.shotsInRounds
            || p_controller.variables.waitInCoverTime > p_controller.variables.coverTime
            || Equals(p_controller.CoverSpot, Vector3.positiveInfinity)) return false;

        return true;
    }
}
