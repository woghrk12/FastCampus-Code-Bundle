using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New TakeCover Action", menuName = "Pluggable AI/Actions/TakeCover")]
public class TakeCoverAction : Action
{
    private readonly int coverMin = 2;
    private readonly int coverMax = 5;

    public override void OnReadyAction(StateController p_controller)
    {
        p_controller.variables.isFeelAlert = false;
        p_controller.variables.waitInCoverTime = 0f;
        if (!Equals(p_controller.CoverSpot, Vector3.positiveInfinity))
        {
            p_controller.enemyAnim.anim.SetBool(FC.AnimatorKey.Crouch, true);
            p_controller.variables.coverTime = Random.Range(coverMin, coverMax);
        }
        else
            p_controller.variables.coverTime = 0.1f;
    }

    public override void Act(StateController p_controller)
    {
        if (!p_controller.isReloading)
            p_controller.variables.waitInCoverTime += Time.deltaTime;

        p_controller.variables.blindEngageTimer += Time.deltaTime;

        if (p_controller.enemyAnim.anim.GetBool(FC.AnimatorKey.Crouch)) Rotate(p_controller);
    }

    private void Rotate(StateController p_controller)
    {
        Vector3 t_dirToTarget = p_controller.personalTarget - p_controller.transform.position;
        if (t_dirToTarget.sqrMagnitude < 0.001f || t_dirToTarget.sqrMagnitude > 1000000.0f) return;

        Quaternion t_targetRotation = Quaternion.LookRotation(t_dirToTarget);
        if (Quaternion.Angle(p_controller.transform.rotation, t_targetRotation) > 5f)
            p_controller.transform.rotation = Quaternion.Slerp(p_controller.transform.rotation, t_targetRotation, 10f * Time.deltaTime);
    }
}
