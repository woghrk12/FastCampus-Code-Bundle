using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ClearShot Decision", menuName = "Pluggable AI/Decisions/ClearShot")]
public class ClearShotDecision : Decision
{
    [Header("Extra Decision")]
    public FocusDecision targetNear;

    private bool HaveClearShot(StateController p_controller)
    {
        Vector3 t_shotOrigin = p_controller.transform.position + Vector3.up * (p_controller.generalStats.aboveCoverHeight + p_controller.nav.radius);
        Vector3 t_shotDir = p_controller.personalTarget - t_shotOrigin;

        bool t_isBlockedShot = Physics.SphereCast(t_shotOrigin, p_controller.nav.radius, t_shotDir, out RaycastHit t_hitInfo, p_controller.nearRadius, p_controller.generalStats.coverMask | p_controller.generalStats.obstacleMask);
        if (!t_isBlockedShot)
        {
            t_isBlockedShot = Physics.Raycast(t_shotOrigin, t_shotDir, out t_hitInfo, t_shotDir.magnitude, p_controller.generalStats.coverMask | p_controller.generalStats.obstacleMask);
            if (t_isBlockedShot) t_isBlockedShot = !(t_hitInfo.transform.root == p_controller.aimTarget.root);
        }

        return t_isBlockedShot;
    }

    public override bool Decide(StateController p_controller)
    {
        return targetNear.Decide(p_controller) || HaveClearShot(p_controller);
    }
}
