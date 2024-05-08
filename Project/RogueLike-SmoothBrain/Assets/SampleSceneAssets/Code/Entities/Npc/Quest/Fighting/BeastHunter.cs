using Map;
using System.Collections;
using UnityEngine;

public class BeastHunter : Quest
{
    int currentNumber = 0;
    int MAX_NUMBER;

    public override void AcceptQuest()
    {
        base.AcceptQuest();

        MAX_NUMBER = 3;

        switch (difficulty)
        {
            case QuestDifficulty.EASY:
                timeToFinishQuest = 600f;
                break;
            case QuestDifficulty.MEDIUM:
                timeToFinishQuest = 450f;
                CorruptionModifierValue += 5;
                break;
            case QuestDifficulty.HARD:
                timeToFinishQuest = 300f;
                CorruptionModifierValue += 10;
                break;
        }
        progressText = $"NB GLORBS KILLED : {currentNumber}/{MAX_NUMBER}";
        Utilities.Hero.OnKill += UpdateCount;
    }

    protected override bool IsQuestFinished()
    {
        return currentNumber >= MAX_NUMBER;
    }

    private void UpdateCount(IDamageable damageable)
    {
        if (!IsQuestFinished() && damageable as IGlorb != null)
        {
            currentNumber++;
            progressText = $"NB GLORBS KILLED : {currentNumber}/{MAX_NUMBER}";
        }
    }

    protected override void ResetQuestValues()
    {
        Utilities.Hero.OnKill -= UpdateCount;
    }
}
