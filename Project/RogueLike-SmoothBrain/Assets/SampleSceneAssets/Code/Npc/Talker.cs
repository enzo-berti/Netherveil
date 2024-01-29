using DialogueSystem.Runtime;
using UnityEngine;

public class Talker : Npc, IInterractable
{
    [Header("Talker parameters")]
    [SerializeField] private DialogueContainer dialogue;

    public void Interract()
    {
        TriggerDialogue();
    }

    private void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }
}
