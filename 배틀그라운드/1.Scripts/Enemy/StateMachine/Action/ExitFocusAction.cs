using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ExitFocus Action", menuName = "Pluggable AI/Actions/ExitFocus")]
public class ExitFocusAction : Action
{
    public override void OnReadyAction(StateController p_controller)
    {
        p_controller.isFocusSight = false;
        p_controller.variables.isFeelAlert = false;
        p_controller.variables.isHearAlert = false;
        p_controller.IsStrafing = false;
        p_controller.nav.destination = p_controller.personalTarget;
        p_controller.nav.speed = 0f;
    }

    public override void Act(StateController p_controller)
    {
        
    }
}
