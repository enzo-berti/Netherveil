
using System.Collections;
using UnityEngine;

public class SpinKiller : Quest
{
    int currentNumber = 0;
    bool asDoAnChargedAttack = false;
    int MAX_NUMBER;

    public override void AcceptQuest()
    {
        base.AcceptQuest();
        switch (difficulty)
        {
            case QuestDifficulty.EASY:
                MAX_NUMBER = 5;
                timeToFinishQuest = 500f;
                break;
            case QuestDifficulty.MEDIUM:
                MAX_NUMBER = 8;
                timeToFinishQuest = 350f;
                CorruptionModifierValue += 5;
                break;
            case QuestDifficulty.HARD:
                MAX_NUMBER = 12;
                timeToFinishQuest = 240f;
                CorruptionModifierValue += 10;
                break;
        }
        progressText = $"NB ENEMIES KILL WITH CHARGED ATTACK : {currentNumber}/{MAX_NUMBER}";
        Utilities.Hero.OnChargedAttack += SetBool;
        Utilities.Hero.OnKill += UpdateCount;
    }

    public override bool IsQuestFinished()
    {
        return currentNumber >= MAX_NUMBER;
    }

    protected override void ResetQuestValues()
    {
        Utilities.Hero.OnChargedAttack -= SetBool;
        Utilities.Hero.OnKill -= UpdateCount;
    }

    private void SetBool(IDamageable damageable, IAttacker attacker)
    {
        asDoAnChargedAttack = true;
    }

    private void UpdateCount(IDamageable damageable)
    {
        if (!IsQuestFinished() && damageable is not IDummy)
        {
            Entity monster = (damageable as Entity);
            if (asDoAnChargedAttack && monster != null && monster.Stats.GetValue(Stat.HP) <= 0)
            {
                currentNumber++;
                progressText = $"NB ENEMIES KILL WITH CHARGED ATTACK : {currentNumber}/{MAX_NUMBER}";
            }
            asDoAnChargedAttack = false;
        }
        QuestUpdated();
    }
}
