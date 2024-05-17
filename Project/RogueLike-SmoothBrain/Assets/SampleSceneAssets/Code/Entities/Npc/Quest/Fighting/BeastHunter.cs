using System.IO;

public class BeastHunter : Quest
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

        MAX_NUMBER = 3;

        switch (difficulty)
        {
            case QuestDifficulty.EASY:
                timeToFinishQuest = 600f;
                break;
            case QuestDifficulty.MEDIUM:
                timeToFinishQuest = 450f;
                CorruptionModifierValue += 5;
                break;
            case QuestDifficulty.HARD:
                timeToFinishQuest = 300f;
                CorruptionModifierValue += 10;
                break;
        }
        progressText = $"NB <size={HudHandler.current.QuestHUD.progressTextSize + 15}><sprite name=\"glorb\"><size={HudHandler.current.QuestHUD.progressTextSize}> KILLED : {currentNumber}/{MAX_NUMBER}";
        Utilities.Hero.OnKill += UpdateCount;
    }

    public override bool IsQuestFinished()
    {
        return currentNumber >= MAX_NUMBER;
    }

    private void UpdateCount(IDamageable damageable)
    {
        if (!IsQuestFinished() && damageable as IGlorb != null)
        {
            currentNumber++;
            progressText = $"NB GLORBS KILLED : {currentNumber}/{MAX_NUMBER}";
        }
        QuestUpdated();
    }

    protected override void ResetQuestValues()
    {
        Utilities.Hero.OnKill -= UpdateCount;
    }
}
