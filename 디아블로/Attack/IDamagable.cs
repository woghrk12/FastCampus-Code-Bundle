using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    bool IsAlive { get; }
    void TakeDamage(int p_damage, GameObject p_hitEffectPrefab);
}
