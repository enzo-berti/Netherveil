using Map;

public class LostRelics : Quest
{
    int currentNumber = 0;
    int MAX_NUMBER;

    public override void AcceptQuest()
    {
        base.AcceptQuest();

        currentNumber = MapUtilities.nbEnterRoomByType[RoomType.Treasure];
        MAX_NUMBER = MapUtilities.nbRoomByType[RoomType.Treasure];
        progressText = $"NB TREASURE ROOM DISCOVERED : {currentNumber}/{MAX_NUMBER}";
        MapUtilities.onEnter += UpdateCount;
    }

    protected override bool IsQuestFinished()
    {
        return currentNumber >= MAX_NUMBER;
    }

    protected override void QuestFinished()
    {
        base.QuestFinished();
        MapUtilities.onEnter -= UpdateCount;
    }

    private void UpdateCount()
    {
        if (!IsQuestFinished() && MapUtilities.currentRoomData.Type == RoomType.Treasure)
        {
            currentNumber = MapUtilities.nbEnterRoomByType[RoomType.Treasure];
            progressText = $"NB TREASURE ROOM DISCOVERED : {currentNumber}/{MAX_NUMBER}";
        }
        QuestUpdated();
    }
}
