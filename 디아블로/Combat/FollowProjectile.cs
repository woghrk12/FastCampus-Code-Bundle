using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowProjectile : Projectile
{
    public float destroyDelay = 5.0f;



    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        StartCoroutine(DestroyParticle(destroyDelay));
    }

    protected override void FixedUpdate()
    {
        if (target)
        {
            Vector3 t_dest = target.transform.position;
            t_dest.y += 1.5f;
            transform.LookAt(t_dest);
        }

        base.FixedUpdate();
    }
}
