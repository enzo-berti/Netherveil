
public class BeastHunter : Quest
{
    int currentNumber = 0;
    readonly int MAX_NUMBER = 15;

    public override void AcceptQuest()
    {
        benedictionOrCorruptionValue = 20;
        progressText = $"NB ENEMIES KILLED : {currentNumber}/{MAX_NUMBER}";
        Hero.OnKill += UpdateCount;
    }

    protected override void QuestFinished()
    {
        base.QuestFinished();
        Hero.OnKill -= UpdateCount;
    }

    private void UpdateCount(IDamageable damageable)
    {
        if (damageable as IGlorb != null)
        {
            currentNumber++;
            progressText = $"NB ENEMIES KILLED : {currentNumber}/{MAX_NUMBER}";
            QuestUpdated();

            if (currentNumber >= MAX_NUMBER)
            {
                QuestFinished();
            }
        }
    }
}
