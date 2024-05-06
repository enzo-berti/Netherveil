
using System.Collections;
using UnityEngine;

public class TakeYourDistance : Quest
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
        progressText = $"NB ENEMIES HIT WITH DISTANCE ATTACK : {currentNumber}/{MAX_NUMBER}";
        Hero.OnSpearAttack += UpdateCount;
    }

    protected override bool IsQuestFinished()
    {
        return currentNumber >= MAX_NUMBER;
    }

    protected override void ResetQuestValues()
    {
        Hero.OnSpearAttack -= UpdateCount;
    }

    private void UpdateCount(IDamageable damageable, IAttacker attacker)
    {
        if (!IsQuestFinished())
        {
            currentNumber++;
            progressText = $"NB ENEMIES HIT WITH DISTANCE ATTACK : {currentNumber}/{MAX_NUMBER}";
        }

        QuestUpdated();
    }
}
