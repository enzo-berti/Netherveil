using DialogueSystem.Runtime;
using UnityEngine;

public class Talker : Npc
{
    [Header("Talker parameters")]
    [SerializeField] private DialogueTree dialogue;

    public override void Interract()
    {
        TriggerDialogue();
    }

    private void TriggerDialogue()
    {
        FindObjectOfType<DialogueTreeRunner>().StartDialogue(dialogue);
    }
}
