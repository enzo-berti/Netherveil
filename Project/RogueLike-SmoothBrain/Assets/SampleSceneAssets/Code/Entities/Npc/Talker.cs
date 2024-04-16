using DialogueSystem.Runtime;
using UnityEngine;

public class Talker : Npc
{
    [Header("Talker parameters")]
    [SerializeField] private DialogueTree dialogue;
    DialogueTreeRunner dialogueTreeRunner;
    Quest quest;
    Hero player;

    protected override void Start()
    {
        base.Start();
        dialogueTreeRunner = FindObjectOfType<DialogueTreeRunner>();
        player = GameObject.FindWithTag("Player").GetComponent<Hero>();
    }

    public override void Interract()
    {
        TriggerDialogue();
        GiveQuest();
    }

    private void GiveQuest()
    {
        quest = Quest.LoadClass(Quest.GetRandomQuestName());
        player.CurrentQuest = quest;
    }

    private void TriggerDialogue()
    {
        if (dialogueTreeRunner.IsStarted)
        {
            dialogueTreeRunner.UpdateDialogue();
        }
        else
        {
            dialogueTreeRunner.StartDialogue(dialogue);
        }
    }
}
