using Map;

public class LostRelics : Quest
{
    int currentNumber = 0;
    int MAX_NUMBER;

    public override void AcceptQuest()
    {
        base.AcceptQuest();

        MAX_NUMBER = RoomUtilities.nbRoomByType[RoomType.Treasure];
        progressText = $"NB TREASURE ROOM DISCOVERED : {currentNumber}/{MAX_NUMBER}";
        RoomUtilities.onEnter += UpdateCount;
    }

    protected override bool IsQuestFinished()
    {
        return currentNumber >= MAX_NUMBER;
    }

    protected override void QuestFinished()
    {
        base.QuestFinished();
        RoomUtilities.onEnter -= UpdateCount;
    }

    private void UpdateCount()
    {
        if (IsQuestFinished())
            return;

        if (RoomUtilities.roomData.Type == RoomType.Treasure)
        {
            currentNumber = RoomUtilities.nbEnterRoomByType[RoomType.Treasure];
            progressText = $"NB TREASURE ROOM DISCOVERED : {currentNumber}/{MAX_NUMBER}";
            QuestUpdated();
        }
    }


}
