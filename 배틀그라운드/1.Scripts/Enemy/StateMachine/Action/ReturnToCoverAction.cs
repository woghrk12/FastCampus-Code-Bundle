using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ReturnToCover Action", menuName = "Pluggable AI/Actions/ReturnToCover")]
public class ReturnToCoverAction : Action
{
    public override void OnReadyAction(StateController p_controller)
    {
        if (!Equals(p_controller.CoverSpot, Vector3.positiveInfinity))
        {
            p_controller.nav.destination = p_controller.CoverSpot;
            p_controller.nav.speed = p_controller.generalStats.chaseSpeed;
            if (Vector3.Distance(p_controller.CoverSpot, p_controller.transform.position) > 0.5f)
                p_controller.enemyAnim.AbortPendingAim();
        }
        else
            p_controller.nav.destination = p_controller.transform.position;
    }

    public override void Act(StateController p_controller)
    {
        if (!Equals(p_controller.CoverSpot, p_controller.transform.position))
            p_controller.isFocusSight = false;
    }
}
