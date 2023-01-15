using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStateController : MonoBehaviour
{
    #region Variables

    public delegate void OnEnterAttackState();
    public delegate void OnExitAttackState();

    public OnEnterAttackState enterAttackStateHandler;
    public OnExitAttackState exitAttackStateHandler;

    public bool IsInAttackState { private set; get; }

    #endregion Variables

    private void Start()
    {
        enterAttackStateHandler = new OnEnterAttackState(EnterAttackState);
        exitAttackStateHandler = new OnExitAttackState(ExitAttackState);
    }

    #region Helper Methods

    public void OnStartOfAttackState()
    {
        IsInAttackState = true;
        enterAttackStateHandler();
    }

    public void OnEndOfAttackState()
    {
        IsInAttackState = false;
        exitAttackStateHandler();
    }

    private void EnterAttackState()
    { 
    
    }

    private void ExitAttackState()
    { 
    
    }

    public void OnCheckAttackCollider(int p_attackIdx)
    {
        GetComponent<IAttackable>()?.OnExecuteAttack(p_attackIdx);
    }

    #endregion Helper Methods
}
