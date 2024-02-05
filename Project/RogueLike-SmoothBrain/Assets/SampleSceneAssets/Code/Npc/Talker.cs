using DialogueSystem.Runtime;
using UnityEngine;

public class Talker : Npc, IInterractable
{
    [Header("Talker parameters")]
    [SerializeField] private DialogueTree dialogue;

    public void Interract()
    {
        TriggerDialogue();
    }

    private void TriggerDialogue()
    {
        FindObjectOfType<DialogueTreeRunner>().StartDialogue(dialogue);
    }
}
