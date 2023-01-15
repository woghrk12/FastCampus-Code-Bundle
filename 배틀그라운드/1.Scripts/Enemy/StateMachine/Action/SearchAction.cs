using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Search Action", menuName = "Pluggable AI/Actions/Search")]
public class SearchAction : Action
{
    public override void OnReadyAction(StateController p_controller)
    {
        p_controller.isFocusSight = false;
        p_controller.enemyAnim.AbortPendingAim();
        p_controller.enemyAnim.anim.SetBool(FC.AnimatorKey.Crouch, false);
        p_controller.CoverSpot = Vector3.positiveInfinity;
    }

    public override void Act(StateController p_controller)
    {
        if (Equals(p_controller.personalTarget, Vector3.positiveInfinity)) p_controller.nav.destination = p_controller.transform.position;
        else
        {
            p_controller.nav.speed = p_controller.generalStats.chaseSpeed;
            p_controller.nav.destination = p_controller.personalTarget;
        }
    }
}
