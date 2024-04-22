using Map;

public class DungeonsLimits : Quest
{
    float currentNumber = 0f;

    public override void AcceptQuest()
    {
        switch (difficulty)
        {
            case QuestDifficulty.EASY:
                progressText = $"EXPLORE THE DUNGEON : {currentNumber}%/70%";
                break;
            case QuestDifficulty.MEDIUM:
                Datas.CorruptionModifierValue += 5;
                progressText = $"EXPLORE THE DUNGEON : {currentNumber}%/85%";
                break;
            case QuestDifficulty.HARD:
                Datas.CorruptionModifierValue += 10;
                progressText = $"EXPLORE THE DUNGEON : {currentNumber}%/100%";
                break;
        }
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
        switch (difficulty)
        {
            case QuestDifficulty.EASY:
                progressText = $"EXPLORE THE DUNGEON : {currentNumber}%/70%";
                if (currentNumber >= 70)
                {
                    QuestFinished();
                }
                break;
            case QuestDifficulty.MEDIUM:
                progressText = $"EXPLORE THE DUNGEON : {currentNumber}%/85%";
                if (currentNumber >= 85)
                {
                    QuestFinished();
                }
                break;
            case QuestDifficulty.HARD:
                progressText = $"EXPLORE THE DUNGEON : {currentNumber}%/100%";
                if (currentNumber >= 100)
                {
                    QuestFinished();
                }
                break;
        }
        QuestUpdated();
    }
}
