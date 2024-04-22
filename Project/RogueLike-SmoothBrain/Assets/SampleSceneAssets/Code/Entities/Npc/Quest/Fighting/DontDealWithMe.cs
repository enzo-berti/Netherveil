
public class DontDealWithMe : Quest
{
    int currentNumber = 0;
    int MAX_NUMBER;

    public override void AcceptQuest()
    {
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
        progressText = $"NB ENEMIES HIT WITH CHARGED ATTACK : {currentNumber}/{MAX_NUMBER}";
        Hero.OnChargedAttack += UpdateCount;
    }

    protected override void QuestFinished()
    {
        base.QuestFinished();
        Hero.OnChargedAttack -= UpdateCount;
    }

    private void UpdateCount(IDamageable damageable, IAttacker attacker)
    {
        currentNumber++;
        progressText = $"NB ENEMIES HIT WITH CHARGED ATTACK : {currentNumber}/{MAX_NUMBER}";
        QuestUpdated();

        if (currentNumber >= MAX_NUMBER)
        {
            QuestFinished();
        }
    }
}
