using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyVariables 
{
    public bool isFeelAlert;
    public bool isHearAlert;
    public bool isAdvanceCoverDecision;
    public bool isRepeatShot;

    public int waitRounds;
    public float waitInCoverTime;
    public float coverTime;
    public float patrolTimer;

    public float shotTimer;
    public float startShotTimer;
    public int curShots;
    public int shotsInRounds;

    public float blindEngageTimer;
}
