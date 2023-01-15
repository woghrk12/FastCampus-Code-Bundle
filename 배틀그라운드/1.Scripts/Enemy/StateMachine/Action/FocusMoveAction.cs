using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New FocusMove Action", menuName = "Pluggable AI/Actions/FocusMove")]
public class FocusMoveAction : Action
{
    public ClearShotDecision clearShotDecision;

    private Vector3 curDest;
    private bool isAligned;

    public override void OnReadyAction(StateController p_controller)
    {
        p_controller.hadClearShot = p_controller.haveClearShot = false;
        curDest = p_controller.nav.destination;
        p_controller.isFocusSight = true;
        isAligned = false;
    }

    public override void Act(StateController p_controller)
    {
        if (!isAligned)
        {
            p_controller.nav.destination = p_controller.personalTarget;
            p_controller.nav.speed = 0f;
            if (p_controller.enemyAnim.angularSpeed == 0f)
            {
                p_controller.IsStrafing = true;
                isAligned = true;
                p_controller.nav.destination = curDest;
                p_controller.nav.speed = p_controller.generalStats.evadeSpeed;
            }
        }
        else
        {
            p_controller.haveClearShot = clearShotDecision.Decide(p_controller);
            if (p_controller.hadClearShot != p_controller.haveClearShot)
            {
                p_controller.IsAiming = p_controller.haveClearShot;

                if (p_controller.haveClearShot && !Equals(curDest, p_controller.CoverSpot))
                    p_controller.nav.destination = p_controller.transform.position;
            }

            p_controller.hadClearShot = p_controller.haveClearShot;
        }
    }
}
