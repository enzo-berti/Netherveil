using DialogueSystem.Runtime;
using UnityEngine;

public class QuestTalker : Npc
{
    [Header("Talker parameters")]
    [SerializeField] protected DialogueTree questDT;
    [SerializeField] protected DialogueTree refusesDialogueDT;
    [SerializeField] protected DialogueTree alreadyHaveQuestDT;
    [SerializeField] protected DialogueTree alreadyDoneQuestDT;
    protected DialogueTreeRunner dialogueTreeRunner;
    protected Hero player;
    public enum TalkerType
    {
        CLERIC,
        SHAMAN
    }

    public enum TalkerGrade
    {
        BOSS,
        APPRENTICE
    }

    [SerializeField] protected TalkerType type;
    [SerializeField] protected TalkerGrade grade;
    public TalkerType Type => type;
    public TalkerGrade Grade => grade;

    protected override void Start()
    {
        base.Start();
        dialogueTreeRunner = FindObjectOfType<DialogueTreeRunner>();
        player = GameObject.FindWithTag("Player").GetComponent<Hero>();
    }

    public override void Interract()
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

        if (PlayerInvestedInOppositeWay())
        {
            dialogue = refusesDialogueDT;
        }
        else if (player.CurrentQuest != null)
        {
            dialogue = alreadyHaveQuestDT;
        }
        else if (player.GetComponent<PlayerController>().DoneQuestQTThiStage)
        {
            dialogue = alreadyDoneQuestDT;
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
