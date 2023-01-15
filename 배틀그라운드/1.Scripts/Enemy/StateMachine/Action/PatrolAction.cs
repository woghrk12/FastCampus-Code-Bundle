using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Patrol Action", menuName = "Pluggable AI/Actions/Patrol")]
public class PatrolAction : Action
{
    public override void OnReadyAction(StateController p_controller)
    {
        p_controller.enemyAnim.AbortPendingAim();
        p_controller.enemyAnim.anim.SetBool(FC.AnimatorKey.Crouch, false);
        p_controller.personalTarget = Vector3.positiveInfinity;
        p_controller.CoverSpot = Vector3.positiveInfinity;
    }

    public override void Act(StateController p_controller) => Patrol(p_controller);

    private void Patrol(StateController p_controller)
    {
        if (p_controller.patrolWayPoints.Count == 0) return;

        p_controller.isFocusSight = false;
        p_controller.nav.speed = p_controller.generalStats.patrolSpeed;
        if (p_controller.nav.remainingDistance <= p_controller.nav.stoppingDistance && !p_controller.nav.pathPending)
        {
            p_controller.variables.patrolTimer += Time.deltaTime;
            if (p_controller.variables.patrolTimer >= p_controller.generalStats.patrolWaitTime)
            {
                p_controller.wayPointIdx = (p_controller.wayPointIdx + 1) % p_controller.patrolWayPoints.Count;
                p_controller.variables.patrolTimer = 0f;
            }
        }

        try
        {
            p_controller.nav.destination = p_controller.patrolWayPoints[p_controller.wayPointIdx].position;
        }
        catch (UnassignedReferenceException)
        {
            Debug.LogWarning("There is no way points", p_controller.gameObject);
            p_controller.patrolWayPoints = new List<Transform> { p_controller.transform };
            p_controller.nav.destination = p_controller.transform.position;
        }
    }
}
