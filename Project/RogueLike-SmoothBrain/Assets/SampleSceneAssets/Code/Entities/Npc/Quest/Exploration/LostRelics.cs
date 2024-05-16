using Map;
using System.IO;

public class LostRelics : Quest
{
    int currentNumber = 0;
    int MAX_NUMBER;

    public override void Save(BinaryWriter writer)
    {
        base.Save(writer);

        writer.Write(currentNumber);
    }

    public override void Load(BinaryReader reader)
    {
        base.Load(reader);

        currentNumber = reader.ReadInt32();
    }

    public override void AcceptQuest()
    {
        base.AcceptQuest();

        currentNumber = MapUtilities.nbEnterRoomByType[RoomType.Treasure];
        MAX_NUMBER = MapUtilities.nbRoomByType[RoomType.Treasure];
        progressText = $"NB TREASURE ROOM DISCOVERED : {currentNumber}/{MAX_NUMBER}";
        MapUtilities.onEnter += UpdateCount;
    }

    public override bool IsQuestFinished()
    {
        return currentNumber >= MAX_NUMBER;
    }

    protected override void ResetQuestValues()
    {
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
