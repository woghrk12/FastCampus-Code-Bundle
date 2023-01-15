using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New FindCover Action", menuName = "Pluggable AI/Actions/FindCover")]
public class FindCoverAction : Action
{
    public override void OnReadyAction(StateController p_controller)
    {
        p_controller.isFocusSight = false;
        p_controller.enemyAnim.AbortPendingAim();
        p_controller.enemyAnim.anim.SetBool(FC.AnimatorKey.Crouch, false);

        ArrayList t_nextCoverData = p_controller.coverLookUp.GetBestCoverSpot(p_controller);
        Vector3 t_potentialCover = (Vector3)t_nextCoverData[1];
        if (Vector3.Equals(t_potentialCover, Vector3.positiveInfinity))
        {
            p_controller.nav.destination = p_controller.transform.position;
            return;
        }
        else if ((p_controller.personalTarget - t_potentialCover).sqrMagnitude < (p_controller.personalTarget - p_controller.CoverSpot).sqrMagnitude
            && !p_controller.IsNearOtherSpot(t_potentialCover, p_controller.nearRadius))
        {
            p_controller.coverHash = (int)t_nextCoverData[0];
            p_controller.CoverSpot = t_potentialCover;
        }

        p_controller.nav.destination = p_controller.CoverSpot;
        p_controller.nav.speed = p_controller.generalStats.evadeSpeed;

        p_controller.variables.curShots = p_controller.variables.shotsInRounds;
    }

    public override void Act(StateController p_controller)
    {
        
    }
}
