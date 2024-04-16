
public class VulcanHunter : Quest
{
    int currentNumber = 0;
    readonly int MAX_NUMBER = 15;

    public override void AcceptQuest()
    {
        progressText = $"NB VULCANS KILLED : {currentNumber}/{MAX_NUMBER}";
        Hero.OnKill += UpdateCount;
    }

    protected override void QuestFinished()
    {
        base.QuestFinished();
        Hero.OnKill -= UpdateCount;
    }

    private void UpdateCount(IDamageable damageable)
    {
        if (damageable as IGorgon != null)
        {
            currentNumber++;
            progressText = $"NB VULCANS KILLED : {currentNumber}/{MAX_NUMBER}";
            QuestUpdated();

            if (currentNumber >= MAX_NUMBER)
            {
                QuestFinished();
            }
        }
    }
}
