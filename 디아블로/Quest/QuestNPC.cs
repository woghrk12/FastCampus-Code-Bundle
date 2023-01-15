using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestNPC : MonoBehaviour, IInteractable
{
    public QuestObject questObject;
    
    public Dialogue readyDialogue;
    public Dialogue acceptedDialogue;
    public Dialogue completedDialogue;

    bool isStartQuestDialogue = false;
    private GameObject interactObj = null;

    private float distance = 2.0f;
    public float Distance => distance;

    private void Start()
    {
        QuestManager.Instance.OnCompleteQuest += OnCompleteQuest;
    }

    public bool Interact(GameObject p_otherObj)
    {
        float t_calcDist = Vector3.Distance(p_otherObj.transform.position, transform.position);
        if (t_calcDist > distance) return false;
        if (isStartQuestDialogue) return false;
        interactObj = p_otherObj;

        DialogueManager.Instance.OnEndDialogue += OnEndDialogue;

        isStartQuestDialogue = true;

        if (questObject.status == EQuestStatus.NONE)
        {
            DialogueManager.Instance.StartDialogue(readyDialogue);
            questObject.status = EQuestStatus.ACCEPTED;
        }
        else if (questObject.status == EQuestStatus.ACCEPTED)
        {
            DialogueManager.Instance.StartDialogue(acceptedDialogue);
        }
        else if (questObject.status == EQuestStatus.COMPLETED)
        {
            DialogueManager.Instance.StartDialogue(completedDialogue);

            // Process reward 

            questObject.status = EQuestStatus.REWARDED;
        }

        return true;
    }

    public void StopInteract(GameObject p_otherObj)
    {
        isStartQuestDialogue = false;
    }

    private void OnEndDialogue() { StopInteract(interactObj); }

    private void OnCompleteQuest(QuestObject p_questObj)
    {
        if (p_questObj.data.id == questObject.data.id && p_questObj.status == EQuestStatus.COMPLETED)
        { 
            // Process NPC effect
        }
    }
}
