using UnityEngine;

public class RealTest : Quest
{
    int currentNumber = 0;
    readonly int MAX_NUMBER = 1;

    public override void AcceptQuest()
    {
        benedictionOrCorruptionValue = 30;
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

        if(currentNumber >= MAX_NUMBER) 
        {
            QuestFinished();
        }
    }
} 
