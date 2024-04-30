
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
                MAX_NUMBER = 2;
                break;
            case QuestDifficulty.MEDIUM:
                MAX_NUMBER = 4;
                Datas.CorruptionModifierValue += 5;
                break;
            case QuestDifficulty.HARD:
                MAX_NUMBER = 6;
                Datas.CorruptionModifierValue += 10;
                break;
        }
        progressText = $"NB ENEMIES KILL WITH CHARGED ATTACK : {currentNumber}/{MAX_NUMBER}";
        Hero.OnChargedAttack += SetBool;
        Hero.OnKill += UpdateCount;
    }

    protected override bool IsQuestFinished()
    {
        return currentNumber >= MAX_NUMBER;
    }

    protected override void QuestFinished()
    {
        base.QuestFinished();
        Hero.OnChargedAttack -= SetBool;
        Hero.OnKill -= UpdateCount;
    }

    private void SetBool(IDamageable damageable, IAttacker attacker)
    {
        asDoAnChargedAttack = true;
    }

    private void UpdateCount(IDamageable damageable)
    {
        if (!IsQuestFinished())
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
