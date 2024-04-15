using DialogueSystem.Runtime;
using UnityEngine;

public class Talker : Npc
{
    [Header("Talker parameters")]
    [SerializeField] private DialogueTree dialogue;
    DialogueTreeRunner dialogueTreeRunner;

    protected override void Start()
    {
        base.Start();
        dialogueTreeRunner = FindObjectOfType<DialogueTreeRunner>();
    }

    public override void Interract()
    {
        TriggerDialogue();
    }

    private void TriggerDialogue()
    {
        if(dialogueTreeRunner.IsStarted)
        {
            dialogueTreeRunner.UpdateDialogue();
        }
        else
        {
            dialogueTreeRunner.StartDialogue(dialogue);
        }
    }
}
