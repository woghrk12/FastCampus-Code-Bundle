using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Decision : ScriptableObject
{
    public delegate bool HandleTargets_Del(StateController p_controller, bool p_hasTargets, Collider[] p_targetsInRadius);

    public abstract bool Decide(StateController p_controller);

    public virtual void OnEnableDecision(StateController p_controller) { }

    public static bool CheckTargetsInRadius(StateController p_controller, float p_radius, HandleTargets_Del p_handleTargets)
    {
        if (p_controller.aimTarget.root.GetComponent<HealthBase>().IsDead) return false;

        Collider[] t_targetInRadius = Physics.OverlapSphere(p_controller.transform.position, p_radius, p_controller.generalStats.targetMask);
        return p_handleTargets(p_controller, t_targetInRadius.Length > 0, t_targetInRadius);
    }
}
