using Map;

public class TheBuriedSecrets : Quest
{
    int currentNumber = 0;
    int MAX_NUMBER;

    public override void AcceptQuest()
    {
        base.AcceptQuest();
        MAX_NUMBER = RoomUtilities.nbRoomByType[RoomType.Secret];
        progressText = $"NB SECRETS ROOM DISCOVERED : {currentNumber}/{MAX_NUMBER}";
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

        if (RoomUtilities.roomData.Type == RoomType.Secret)
        {
            currentNumber = RoomUtilities.nbEnterRoomByType[RoomType.Secret];
            progressText = $"NB SECRETS ROOM DISCOVERED : {currentNumber}/{MAX_NUMBER}";
            QuestUpdated();
        }
    }   
}
