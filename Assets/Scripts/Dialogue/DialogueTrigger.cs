using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    public int Index { get; set; }
    private string _setDialogue = "";
    public string SetDialogue
    {
        get => _setDialogue;
        set
        {
            _setDialogue = value;
            LangDialogue(Index);
        }
    }

    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }

    private void LangDialogue(int index)
    {
        dialogue.sentences[index] = _setDialogue;
    }
}
