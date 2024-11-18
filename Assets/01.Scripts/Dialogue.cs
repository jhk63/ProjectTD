using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    public GameObject dialoguePanel;

    public string[] dialogues;
    private int index = 0;
    private int dialoguesCount;
    public TMP_Text dialogueText;

    public UnityEvent onDialogueStart;
    public UnityEvent onDialogueEnd;

    public bool isEnd = false;

    private void Start()
    {
        dialoguePanel.SetActive(false);
        dialoguesCount = dialogues.Length;
        // Debug.Log(dialoguesCount);
    }

    public void StartDialogue()
    {
        if (dialoguesCount == 0)
        {
            dialoguePanel.SetActive(false);
            return;
        }

        onDialogueStart.Invoke();
        dialoguePanel.SetActive(true);
        dialogueText.text = dialogues[index];
    }

    public void NextDialogue()
    {
        if (index >= dialoguesCount - 1)
        {
            EndDialogue();
            return;
        }

        index++;
        dialogueText.text = dialogues[index];
    }

    public void EndDialogue()
    {
        isEnd = true;
        onDialogueEnd.Invoke();
        dialoguePanel.SetActive(false);
    }
}
