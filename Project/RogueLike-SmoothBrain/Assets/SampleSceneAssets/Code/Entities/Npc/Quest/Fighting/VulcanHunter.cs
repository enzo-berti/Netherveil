
public class VulcanHunter : Quest
{
    int currentNumber = 0;
    int MAX_NUMBER;

    public override void AcceptQuest()
    {
        base.AcceptQuest();
        switch (difficulty)
        {
            case QuestDifficulty.EASY:
                MAX_NUMBER = 2;
                break;
            case QuestDifficulty.MEDIUM:
                MAX_NUMBER = 4;
                CorruptionModifierValue += 5;
                break;
            case QuestDifficulty.HARD:
                MAX_NUMBER = 6;
                CorruptionModifierValue += 10;
                break;
        }
        progressText = $"NB VULCANS KILLED : {currentNumber}/{MAX_NUMBER}";
        Hero.OnKill += UpdateCount;
    }

    protected override bool IsQuestFinished()
    {
        return currentNumber >= MAX_NUMBER;
    }

    protected override void ResetQuestValues()
    {
        Hero.OnKill -= UpdateCount;
    }

    private void UpdateCount(IDamageable damageable)
    {
        if (!IsQuestFinished() && damageable as IGorgon != null)
        {
            currentNumber++;
            progressText = $"NB VULCANS KILLED : {currentNumber}/{MAX_NUMBER}";
        }
        QuestUpdated();
    }
}
