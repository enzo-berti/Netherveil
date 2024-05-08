
using System.Collections;
using UnityEngine;

public class KitingMyDearLove : Quest 
{
    int currentNumber = 0;
    bool asDoAnDistanceAttack = false;
    int MAX_NUMBER;

    public override void AcceptQuest()
    {
        base.AcceptQuest();
        switch (difficulty)
        {
            case QuestDifficulty.EASY:
                MAX_NUMBER = 5;
                timeToFinishQuest = 400f;
                break;
            case QuestDifficulty.MEDIUM:
                MAX_NUMBER = 8;
                timeToFinishQuest = 250f;
                CorruptionModifierValue += 5;
                break;
            case QuestDifficulty.HARD:
                MAX_NUMBER = 10;
                timeToFinishQuest = 180f;
                CorruptionModifierValue += 10;
                break;
        }
        progressText = $"NB ENEMIES KILL WITH DISTANCE ATTACK : {currentNumber}/{MAX_NUMBER}";
        Utilities.Hero.OnSpearAttack += SetBool;
        Utilities.Hero.OnKill += UpdateCount;
    }

    public override bool IsQuestFinished()
    {
        return currentNumber >= MAX_NUMBER;
    }

    protected override void ResetQuestValues()
    {
        Utilities.Hero.OnSpearAttack -= SetBool;
        Utilities.Hero.OnKill -= UpdateCount;
    }

    private void SetBool(IDamageable damageable, IAttacker attacker)
    {
        asDoAnDistanceAttack = true;
    }

    private void UpdateCount(IDamageable damageable)
    {
        if (!IsQuestFinished() && damageable is not IDummy)
        {
            Entity monster = (damageable as Entity);
            if (asDoAnDistanceAttack && monster != null && monster.Stats.GetValue(Stat.HP) <= 0)
            {
                currentNumber++;
                progressText = $"NB ENEMIES KILL WITH DISTANCE ATTACK : {currentNumber}/{MAX_NUMBER}";
            }
            asDoAnDistanceAttack = false;
        }
        QuestUpdated();
    }
} 
