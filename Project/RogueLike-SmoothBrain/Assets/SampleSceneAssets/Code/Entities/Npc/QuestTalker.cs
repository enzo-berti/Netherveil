using DialogueSystem.Runtime;
using UnityEngine;

public class QuestTalker : Npc
{
    [Header("Talker parameters")]
    [SerializeField] protected DialogueTree questDT;
    [SerializeField] protected DialogueTree refusesDialogueDT;
    [SerializeField] protected DialogueTree alreadyHaveQuestDT;
    protected DialogueTreeRunner dialogueTreeRunner;
    protected Hero player;
    public enum TalkerType
    {
        CLERIC,
        SHAMAN
    }

    [SerializeField] protected TalkerType type;
    public TalkerType Type => type;

    protected override void Start()
    {
        base.Start();
        dialogueTreeRunner = FindObjectOfType<DialogueTreeRunner>();
        player = GameObject.FindWithTag("Player").GetComponent<Hero>();
    }

    public override void Interract()
    {
        TriggerDialogue();
    }

    private void TriggerDialogue()
    {
        if (dialogueTreeRunner.IsStarted)
        {
            dialogueTreeRunner.UpdateDialogue();
        }
        else
        {
            StartDialogue();
        }
    }

    protected virtual void StartDialogue()
    {
        DialogueTree dialogue = questDT;

        if (player.CurrentQuest != null)
        {
            dialogue = alreadyHaveQuestDT;
        }
        else if (PlayerInvestedInOppositeWay())
        {
            dialogue = refusesDialogueDT;
        }

        dialogueTreeRunner.StartDialogue(dialogue, this);
    }

    protected bool PlayerInvestedInOppositeWay()
    {
        if(type == TalkerType.CLERIC && player.Stats.GetValue(Stat.CORRUPTION) >= player.STEP_VALUE)
        {
            return true;
        }
        else if (type == TalkerType.SHAMAN && player.Stats.GetValue(Stat.CORRUPTION) <= -player.STEP_VALUE)
        {
            return true;
        }
        return false;
    }
}
