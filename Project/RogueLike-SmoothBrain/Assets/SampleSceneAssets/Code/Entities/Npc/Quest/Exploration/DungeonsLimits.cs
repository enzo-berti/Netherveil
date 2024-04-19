using Map;

public class DungeonsLimits : Quest
{
    float currentNumber = 0f;

    public override void AcceptQuest()
    {
        progressText = $"EXPLORE THE DUNGEON : {currentNumber}%/100%";
        RoomUtilities.enterEvents += UpdateCount;
    }

    protected override void QuestFinished()
    {
        base.QuestFinished();
        RoomUtilities.enterEvents -= UpdateCount;
    }

    private void UpdateCount()
    {
        currentNumber = RoomUtilities.nbEnterRoomByType[RoomType.Normal] / RoomUtilities.nbRoomByType[RoomType.Normal] * 100f;
        progressText = $"EXPLORE THE DUNGEON : {currentNumber}%/100%";
        QuestUpdated();

        if (currentNumber >= 100)
        {
            QuestFinished();
        }
    }
}
