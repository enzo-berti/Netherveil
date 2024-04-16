
public class TakeYourDistance : Quest
{
    int currentNumber = 0;
    readonly int MAX_NUMBER = 20;

    public override void AcceptQuest()
    {
        progressText = $"NB ENEMIES HIT WITH DISTANCE ATTACK: {currentNumber}/{MAX_NUMBER}";
        Hero.OnSpearAttack += UpdateCount;
    }

    protected override void QuestFinished()
    {
        base.QuestFinished();
        Hero.OnSpearAttack -= UpdateCount;
    }

    private void UpdateCount(IDamageable damageable, IAttacker attacker)
    {
        currentNumber++;
        progressText = $"NB ENEMIES HIT WITH DISTANCE ATTACK : {currentNumber}/{MAX_NUMBER}";
        QuestUpdated();

        if (currentNumber >= MAX_NUMBER)
        {
            QuestFinished();
        }
    }
}
