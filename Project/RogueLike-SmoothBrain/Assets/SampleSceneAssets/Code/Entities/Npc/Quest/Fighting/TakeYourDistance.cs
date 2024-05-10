
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
                timeToFinishQuest = 350f;
                break;
            case QuestDifficulty.MEDIUM:
                MAX_NUMBER = 15;
                timeToFinishQuest = 260f;
                CorruptionModifierValue += 5;
                break;
            case QuestDifficulty.HARD:
                MAX_NUMBER = 20;
                timeToFinishQuest = 150f;
                CorruptionModifierValue += 10;
                break;
        }
        progressText = $"NB ENEMIES HIT WITH DISTANCE ATTACK : {currentNumber}/{MAX_NUMBER}";
        Utilities.Hero.OnSpearAttack += UpdateCount;
    }

    public override bool IsQuestFinished()
    {
        return currentNumber >= MAX_NUMBER;
    }

    protected override void ResetQuestValues()
    {
        Utilities.Hero.OnSpearAttack -= UpdateCount;
    }

    private void UpdateCount(IDamageable damageable, IAttacker attacker)
    {
        if (!IsQuestFinished() && damageable is not IDummy)
        {
            currentNumber++;
            progressText = $"NB ENEMIES HIT WITH DISTANCE ATTACK : {currentNumber}/{MAX_NUMBER}";
        }

        QuestUpdated();
    }
}
