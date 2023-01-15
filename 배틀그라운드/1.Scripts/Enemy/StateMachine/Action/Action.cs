using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Action : ScriptableObject
{
    public abstract void Act(StateController p_controller);

    public virtual void OnReadyAction(StateController p_controller) { }
}
