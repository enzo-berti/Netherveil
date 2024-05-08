
using System.Collections;
using UnityEngine;

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
                timeToFinishQuest = 600f;
                MAX_NUMBER = 2;
                break;
            case QuestDifficulty.MEDIUM:
                MAX_NUMBER = 4;
                timeToFinishQuest = 450f;
                CorruptionModifierValue += 5;
                break;
            case QuestDifficulty.HARD:
                MAX_NUMBER = 6;
                timeToFinishQuest = 300f;
                CorruptionModifierValue += 10;
                break;
        }
        progressText = $"NB GORGONS KILLED : {currentNumber}/{MAX_NUMBER}";
        Utilities.Hero.OnKill += UpdateCount;
    }

    protected override bool IsQuestFinished()
    {
        return currentNumber >= MAX_NUMBER;
    }

    protected override void ResetQuestValues()
    {
        Utilities.Hero.OnKill -= UpdateCount;
    }

    private void UpdateCount(IDamageable damageable)
    {
        if (!IsQuestFinished() && damageable as IGorgon != null)
        {
            currentNumber++;
            progressText = $"NB GORGONS KILLED : {currentNumber}/{MAX_NUMBER}";
        }
        QuestUpdated();
    }
}
