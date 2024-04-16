using DialogueSystem.Runtime;
using UnityEngine;

public class Talker : Npc
{
    [Header("Talker parameters")]
    [SerializeField] private DialogueTree dialogue;
    DialogueTreeRunner dialogueTreeRunner;
    Quest quest;
    Hero player;
    public enum TalkerType
    {
        CLERIC,
        SHAMAN
    }

    [SerializeField] TalkerType type;

    protected override void Start()
    {
        base.Start();
        dialogueTreeRunner = FindObjectOfType<DialogueTreeRunner>();
        player = GameObject.FindWithTag("Player").GetComponent<Hero>();
        dialogueTreeRunner.EventManager.AddListener("GiveQuest", GiveQuest);
    }

    private void OnDestroy()
    {
        dialogueTreeRunner.EventManager.RemoveListener("GiveQuest");
    }

    public override void Interract()
    {
        TriggerDialogue();
        //GiveQuest();
    }

    private void GiveQuest()
    {
        if(dialogueTreeRunner.TalkerNPC == this)
        {
            quest = Quest.LoadClass(Quest.GetRandomQuestName(), type);
            player.CurrentQuest = quest;
        }
    }

    private void TriggerDialogue()
    {
        if (dialogueTreeRunner.IsStarted)
        {
            dialogueTreeRunner.UpdateDialogue();
        }
        else
        {
            dialogueTreeRunner.StartDialogue(dialogue, this);
        }
    }
}
