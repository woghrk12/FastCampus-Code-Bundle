using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private static QuestManager instance;
    public static QuestManager Instance => instance;

    public QuestDatabaseObject questDatabase;

    public event Action<QuestObject> OnCompleteQuest;

    private void Awake()
    {
        instance = this;
    }

    public void ProcessQuest(EQuestType p_type, int p_targetId) 
    {
        foreach (QuestObject t_questObject in questDatabase.questObjects)
        {
            if (t_questObject.status == EQuestStatus.ACCEPTED
                && t_questObject.data.type == p_type
                && t_questObject.data.targetID == p_targetId)
            {
                t_questObject.data.completedCount++;
                if (t_questObject.data.completedCount >= t_questObject.data.count)
                {
                    t_questObject.status = EQuestStatus.COMPLETED;
                    OnCompleteQuest?.Invoke(t_questObject);
                }
            }
        }
    }
}
