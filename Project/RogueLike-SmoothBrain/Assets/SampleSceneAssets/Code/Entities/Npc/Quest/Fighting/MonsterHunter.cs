
public class MonsterHunter : Quest
{
    int currentNumber = 0;
    int MAX_NUMBER;

    public override void AcceptQuest()
    {
        base.AcceptQuest();
        switch (difficulty)
        {
            case QuestDifficulty.EASY:
                MAX_NUMBER = 10;
                break;
            case QuestDifficulty.MEDIUM:
                MAX_NUMBER = 15;
                Datas.CorruptionModifierValue += 5;
                break;
            case QuestDifficulty.HARD:
                MAX_NUMBER = 20;
                Datas.CorruptionModifierValue += 10;
                break;
        } 
        progressText = $"NB MONSTERS KILLED : {currentNumber}/{MAX_NUMBER}";
        Hero.OnKill += UpdateCount;
    }

    protected override bool IsQuestFinished()
    {
        return currentNumber >= MAX_NUMBER;
    }

    protected override void QuestFinished()
    {
        base.QuestFinished();
        Hero.OnKill -= UpdateCount;
    }

    private void UpdateCount(IDamageable damageable)
    {
        if (!IsQuestFinished())
        {
            currentNumber++;
            progressText = $"NB MONSTERS KILLED : {currentNumber}/{MAX_NUMBER}";
        }
        QuestUpdated();
    }
}
