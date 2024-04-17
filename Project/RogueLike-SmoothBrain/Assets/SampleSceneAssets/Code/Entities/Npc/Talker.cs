using DialogueSystem.Runtime;
using UnityEngine;

public class Talker : Npc
{
    [Header("Talker parameters")]
    [SerializeField] private DialogueTree questDT;
    [SerializeField] private DialogueTree refusesDialogueDT;
    [SerializeField] private DialogueTree alreadyHaveQuestDT;
    DialogueTreeRunner dialogueTreeRunner;
    Hero player;
    public enum TalkerType
    {
        CLERIC,
        SHAMAN
    }

    [SerializeField] TalkerType type;
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
        //GiveQuest();
    }

    private void TriggerDialogue()
    {
        if (dialogueTreeRunner.IsStarted)
        {
            dialogueTreeRunner.UpdateDialogue();
        }
        else
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
    }

    private bool PlayerInvestedInOppositeWay()
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
