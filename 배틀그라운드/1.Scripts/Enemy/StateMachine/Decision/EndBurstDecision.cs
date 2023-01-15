using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New EndBurst Decision", menuName = "Pluggable AI/Decisions/EndBurst")]
public class EndBurstDecision : Decision
{
    public override bool Decide(StateController p_controller)
    {
        return p_controller.variables.curShots >= p_controller.variables.shotsInRounds;
    }
}
