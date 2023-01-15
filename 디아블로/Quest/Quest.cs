using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EQuestType
{ 
    DESTROYENEMY,
    ACQUIREITEM,
}

[Serializable]
public class Quest 
{
    public int id;

    public EQuestType type;
    public int targetID;
    public int count;
    public int completedCount;

    public int rewardExp;
    public int rewardGold;
    public int rewardItemId;

    public string title;
    public string description;
}
