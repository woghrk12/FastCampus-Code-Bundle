using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    private static DialogueManager instance;
    public static DialogueManager Instance => instance;

    public Text nameText;
    public Text dialogueText;

    public Animator anim = null;

    private Queue<string> sentences = new Queue<string>();

    public event Action OnStartDialogue;
    public event Action OnEndDialogue;

    private void Awake()
    {
        instance = this;
    }

    public void StartDialogue(Dialogue p_dialogue)
    {
        OnStartDialogue?.Invoke();

        anim?.SetBool("IsOpen", true);

        nameText.text = p_dialogue.name;

        sentences.Clear();
        foreach (string t_sentence in p_dialogue.sentences)
        {
            sentences.Enqueue(t_sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence(Text p_continueText = null)
    {
        if (sentences.Count <= 0) 
        {
            EndDialogue();
            return;
        }

        if (p_continueText != null) p_continueText.text = sentences.Count > 1 ? "Continue>>" : "End";

        string t_sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(t_sentence));
    }

    private IEnumerator TypeSentence(string p_sentence)
    {
        dialogueText.text = string.Empty;

        yield return new WaitForSeconds(0.25f);

        foreach (char t_letter in p_sentence.ToCharArray())
        {
            dialogueText.text += t_letter;
            yield return null;
        }
    }

    private void EndDialogue()
    {
        anim?.SetBool("IsOpen", false);
        OnEndDialogue?.Invoke();
    }
}
