
using Map;

public class LostRelics : Quest
{
    int currentNumber = 0;
    int MAX_NUMBER;

    public override void AcceptQuest()
    {
        MAX_NUMBER = RoomUtilities.nbRoomByType[RoomType.Treasure];
        progressText = $"NB TREASURE ROOM DISCOVERED : {currentNumber}/{MAX_NUMBER}";
        RoomUtilities.enterEvents += UpdateCount;
    }

    protected override void QuestFinished()
    {
        base.QuestFinished();
        RoomUtilities.enterEvents -= UpdateCount;
    }

    private void UpdateCount()
    {
        if (RoomUtilities.roomData.Type == RoomType.Treasure)
        {
            currentNumber = RoomUtilities.nbEnterRoomByType[RoomType.Treasure];
            progressText = $"NB TREASURE ROOM DISCOVERED : {currentNumber}/{MAX_NUMBER}";
            QuestUpdated();

            if (currentNumber >= MAX_NUMBER)
            {
                QuestFinished();
            }
        }
    }


}
