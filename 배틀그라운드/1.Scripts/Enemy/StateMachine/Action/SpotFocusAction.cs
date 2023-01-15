using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New SpotFocus Action", menuName = "Pluggable AI/Actions/SpotFocus")]
public class SpotFocusAction : Action
{
    public override void Act(StateController p_controller)
    {
        p_controller.nav.destination = p_controller.personalTarget;
        p_controller.nav.speed = 0f;
    }
}
