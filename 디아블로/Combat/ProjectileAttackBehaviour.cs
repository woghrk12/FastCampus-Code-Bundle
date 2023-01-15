using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAttackBehaviour : AttackBehaviour
{
    public override void ExecuteAttack(GameObject target = null, Transform startPoint = null)
    {
        if (target == null) return;

        Vector3 t_projectilePos = startPoint?.position ?? transform.position;
        if (effectPrefab)
        {
            GameObject t_projectileGo = Instantiate(effectPrefab, t_projectilePos, Quaternion.identity);
            t_projectileGo.transform.forward = transform.forward;

            Projectile t_projectile = t_projectileGo.GetComponent<Projectile>();
            if (t_projectile)
            {
                t_projectile.owner = this.gameObject;
                t_projectile.target = target;
                t_projectile.attackBehaviour = this;
            }
        }

        calcCoolTime = 0.0f;
    }
}
