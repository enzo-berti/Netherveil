using Map;

public class DungeonsLimits : Quest
{
    float currentNumber = 0f;
    float MAX_NUMBER;
    float poucentageAmout;

    public override void AcceptQuest()
    {
        MAX_NUMBER = RoomUtilities.NbRoom;
        poucentageAmout = 100f / MAX_NUMBER;
        progressText = $"EXPLORE THE DUNGEON : {currentNumber}%/100%";
        RoomUtilities.allEnemiesDeadEvents += UpdateCount;
    }

    protected override void QuestFinished()
    {
        base.QuestFinished();
        RoomUtilities.allEnemiesDeadEvents -= UpdateCount;
    }

    private void UpdateCount()
    {
        currentNumber += poucentageAmout;
        progressText = $"EXPLORE THE DUNGEON : {currentNumber}%/{MAX_NUMBER}%";
        QuestUpdated();

        if (currentNumber >= 100)
        {
            QuestFinished();
        }
    }


}
