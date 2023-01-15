using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable
{
    AttackBehaviour CurrentAttackBehaviour { set; get; }
    void OnExecuteAttack(int p_attackIdx);
}
