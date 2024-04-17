using DialogueSystem.Runtime;

public class QuestTalkerApprentice : QuestTalker
{
    protected override void StartDialogue()
    {
        DialogueTree dialogue = refusesDialogueDT;

        if (player.CurrentQuest != null)
        {
            dialogue = alreadyHaveQuestDT;
        }
        else if (PlayerInvestedInOppositeWay())
        {
            dialogue = questDT;
        }

        dialogueTreeRunner.StartDialogue(dialogue, this);
    }
}
