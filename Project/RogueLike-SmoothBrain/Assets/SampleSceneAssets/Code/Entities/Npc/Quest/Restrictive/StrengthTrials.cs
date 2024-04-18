using UnityEngine; 
 
public class StrengthTrials : Quest 
{
    bool isQuestFinished = false;
    public override void AcceptQuest()
    {
        if(Utilities.Inventory.PassiveItems.Count > 0)
            Utilities.Inventory.PassiveItems[^1].DisableUntil(() => isQuestFinished);
    }
    protected override void QuestFinished()
    {
        base.QuestFinished();
        isQuestFinished = true;
    }
} 
