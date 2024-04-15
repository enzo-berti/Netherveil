using DialogueSystem.Runtime;
using UnityEngine;

public class Talker : Npc
{
    [Header("Talker parameters")]
    [SerializeField] private DialogueTree dialogue;
    DialogueTreeRunner dialogueTreeRunner;
    Quest quest;

    protected override void Start()
    {
        base.Start();
        dialogueTreeRunner = FindObjectOfType<DialogueTreeRunner>();
    }

    public override void Interract()
    {
        TriggerDialogue();
        quest = Quest.LoadClass(Quest.GetRandomQuestName());
        //add to player and update UI
        Debug.Log(quest.idItemName);
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
