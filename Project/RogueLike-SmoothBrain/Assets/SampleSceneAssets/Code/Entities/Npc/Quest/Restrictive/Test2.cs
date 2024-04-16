public class Test2 : Quest
{
    int currentNumber = 0;
    readonly int MAX_NUMBER = 50;

    public override void AcceptQuest()
    {
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
        currentNumber++;
        progressText = $"NB ENEMIES KILLED : {currentNumber}/{MAX_NUMBER}";
        QuestUpdated();
    }
} 
