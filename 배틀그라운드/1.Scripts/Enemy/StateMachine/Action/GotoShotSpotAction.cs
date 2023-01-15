using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New GotoShotSpot Action", menuName = "Pluggable AI/Actions/GotoShotSpot")]
public class GotoShotSpotAction : Action
{
    public override void OnReadyAction(StateController p_controller)
    {
        p_controller.isFocusSight = false;
        p_controller.nav.destination = p_controller.personalTarget;
        p_controller.nav.speed = p_controller.generalStats.chaseSpeed;
        p_controller.enemyAnim.AbortPendingAim();
    }

    public override void Act(StateController p_controller) { } 
}
