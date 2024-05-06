
using System.Collections;
using UnityEngine;

public class DontDealWithMe : Quest
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
                timeToFinishQuest = 600f;
                break;
            case QuestDifficulty.MEDIUM:
                MAX_NUMBER = 15;
                timeToFinishQuest = 450f;
                CorruptionModifierValue += 5;
                break;
            case QuestDifficulty.HARD:
                MAX_NUMBER = 20;
                timeToFinishQuest = 300f;
                CorruptionModifierValue += 10;
                break;
        }
        progressText = $"NB ENEMIES HIT WITH CHARGED ATTACK : {currentNumber}/{MAX_NUMBER}";
        Hero.OnChargedAttack += UpdateCount;
    }

    protected override bool IsQuestFinished()
    {
        return currentNumber >= MAX_NUMBER;
    }

    protected override void ResetQuestValues()
    {
        Hero.OnChargedAttack -= UpdateCount;
    }

    private void UpdateCount(IDamageable damageable, IAttacker attacker)
    {
        if (!IsQuestFinished())
        {
            currentNumber++;
            progressText = $"NB ENEMIES HIT WITH CHARGED ATTACK : {currentNumber}/{MAX_NUMBER}";
        }

        QuestUpdated();
    }
}
