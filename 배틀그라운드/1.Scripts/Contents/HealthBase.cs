using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DamageInfo
{
    public Vector3 position, direction;
    public float damage;
    public Collider bodyPart;
    public GameObject hitEffect;

    public DamageInfo(Vector3 p_position, Vector3 p_direction, float p_damage, Collider p_bodyPart = null, GameObject p_hitEffect = null)
    {
        position = p_position;
        direction = p_direction;
        damage = p_damage;
        bodyPart = p_bodyPart;
        hitEffect = p_hitEffect;
    }
}

public class HealthBase : MonoBehaviour
{
    protected Animator anim;

    [HideInInspector] public bool IsDead;

    public virtual void TakeDamage(Vector3 p_pos, Vector3 p_dir, float p_damage, Collider p_bodyPart = null, GameObject p_hitEffect = null) { }
    public void OnDamage(DamageInfo p_damageInfo) => TakeDamage(p_damageInfo.position, p_damageInfo.direction, p_damageInfo.damage, p_damageInfo.bodyPart, p_damageInfo.hitEffect);
}
