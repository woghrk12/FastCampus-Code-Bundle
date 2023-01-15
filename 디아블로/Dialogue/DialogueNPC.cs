using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueNPC : MonoBehaviour, IInteractable
{
    [SerializeField] private Dialogue dialogue;

    bool isStartDialogue = false;

    private GameObject interactObj;

    [SerializeField] private float distance = 2.0f;
    public float Distance => distance;

    public bool Interact(GameObject p_otherObj)
    {
        float t_calcDist = Vector3.Distance(p_otherObj.transform.position, transform.position);
        if (t_calcDist > distance) return false;
        if (isStartDialogue) return false;
        interactObj = p_otherObj;

        DialogueManager.Instance.OnEndDialogue += OnEndDialogue;
        isStartDialogue = true;
        DialogueManager.Instance.StartDialogue(dialogue);
        return true;
    }

    public void StopInteract(GameObject p_otherObj)
    {
        isStartDialogue = false;
    }

    private void OnEndDialogue()
    {
        StopInteract(interactObj);
    }
}
