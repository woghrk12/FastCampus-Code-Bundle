using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New FeelAlert Decision", menuName = "Pluggable AI/Decisions/FeelAlert")]
public class FeelAlertDecision : Decision
{
    public override bool Decide(StateController p_controller)
    {
        return p_controller.variables.isFeelAlert;
    }
}
