using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stats", menuName = "Pluggable AI/GeneralStats")]
public class GeneralStats : ScriptableObject
{
    [Header("General")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 5f;
    public float evadeSpeed = 15f;
    public float patrolWaitTime = 2f;
    
    [Header("Animation")]
    public LayerMask obstacleMask;

    public float angleDeadZone = 5f;
    public float speedDampTime = 0.4f;
    public float angularSpeedDampTime = 0.2f;
    public float angleResponseTime = 0.2f;

    [Header("Cover")]
    public float aboveCoverHeight = 1.5f;
    public LayerMask coverMask;
    public LayerMask shotMask;
    public LayerMask targetMask;
}
