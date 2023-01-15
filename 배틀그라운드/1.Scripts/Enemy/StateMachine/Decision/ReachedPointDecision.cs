using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ReachedPoint Decision", menuName = "Pluggable AI/Decisions/ReachedPoint")]
public class ReachedPointDecision : Decision
{
    public override bool Decide(StateController p_controller)
    {
        if (!Application.isPlaying) return false;

        if (p_controller.nav.remainingDistance <= p_controller.nav.stoppingDistance && !p_controller.nav.pathPending) return true;

        return false;
    }
}
