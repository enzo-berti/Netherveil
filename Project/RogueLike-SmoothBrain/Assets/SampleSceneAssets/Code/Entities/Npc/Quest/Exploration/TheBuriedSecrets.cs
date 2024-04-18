
using Map;

public class TheBuriedSecrets : Quest
{
    int currentNumber = 0;
    int MAX_NUMBER;

    public override void AcceptQuest()
    {
        MAX_NUMBER = RoomUtilities.nbRoomByType[RoomType.Secret];
        progressText = $"NB SECRETS ROOM DISCOVERED : {currentNumber}/{MAX_NUMBER}";
        RoomUtilities.EnterEvents += UpdateCount;
    }

    protected override void QuestFinished()
    {
        base.QuestFinished();
        RoomUtilities.EnterEvents -= UpdateCount;
    }

    private void UpdateCount()
    {
        if (RoomUtilities.roomData.Type == RoomType.Secret)
        {
            currentNumber = RoomUtilities.nbEnterRoomByType[RoomType.Secret];
            progressText = $"NB SECRETS ROOM DISCOVERED : {currentNumber}/{MAX_NUMBER}";
            QuestUpdated();

            if (currentNumber >= MAX_NUMBER)
            {
                QuestFinished();
            }
        }
    }
}
