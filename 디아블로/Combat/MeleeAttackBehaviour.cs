using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackBehaviour : AttackBehaviour
{
    public ManualCollision attackCollision;

    public override void ExecuteAttack(GameObject target = null, Transform startPoint = null)
    {
        Collider[] t_colliders = attackCollision?.CheckOverlapBox(targetMask);

        foreach (Collider t_collider in t_colliders)
        {
            t_collider.gameObject.GetComponent<IDamagable>()?.TakeDamage(damage, effectPrefab);
        }
    }
}
