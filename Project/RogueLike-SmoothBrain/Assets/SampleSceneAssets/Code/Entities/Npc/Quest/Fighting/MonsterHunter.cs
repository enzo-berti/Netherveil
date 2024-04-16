
public class MonsterHunter : Quest
{
    int currentNumber = 0;
    readonly int MAX_NUMBER = 15;

    public override void AcceptQuest()
    {
        progressText = $"NB MONSTERS KILLED : {currentNumber}/{MAX_NUMBER}";
        Hero.OnKill += UpdateCount;
    }

    protected override void QuestFinished()
    {
        base.QuestFinished();
        Hero.OnKill -= UpdateCount;
    }

    private void UpdateCount(IDamageable damageable)
    {
        currentNumber++;
        progressText = $"NB MONSTERS KILLED : {currentNumber}/{MAX_NUMBER}";
        QuestUpdated();

        if (currentNumber >= MAX_NUMBER)
        {
            QuestFinished();
        }
    }
}
