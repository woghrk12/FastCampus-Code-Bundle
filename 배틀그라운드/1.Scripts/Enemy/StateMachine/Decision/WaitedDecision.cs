using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Waited Decision", menuName = "Pluggable AI/Decisions/Waited")]
public class WaitedDecision : Decision
{
    public float maxTimeToWait;
    private float timeToWait;
    private float startTime;

    public override void OnEnableDecision(StateController p_controller)
    {
        timeToWait = Random.Range(0, maxTimeToWait);
        startTime = Time.time;
    }

    public override bool Decide(StateController p_controller)
    {
        return (Time.time - startTime) >= timeToWait;
    }
}
