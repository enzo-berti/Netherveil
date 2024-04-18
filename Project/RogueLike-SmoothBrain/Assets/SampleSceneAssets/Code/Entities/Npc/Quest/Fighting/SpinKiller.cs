
using UnityEngine;

public class SpinKiller : Quest
{
    int currentNumber = 0;
    bool asDoAnChargedAttack = false;
    readonly int MAX_NUMBER = 5;

    public override void AcceptQuest()
    {
        progressText = $"NB ENEMIES KILL WITH CHARGED ATTACK : {currentNumber}/{MAX_NUMBER}";
        Hero.OnChargedAttack += SetBool;
        Hero.OnKill += UpdateCount;
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
        Entity monster = (damageable as Entity);
        if (asDoAnChargedAttack && monster != null && monster.Stats.GetValue(Stat.HP) <= 0)
        {
            currentNumber++;
            progressText = $"NB ENEMIES KILL WITH CHARGED ATTACK : {currentNumber}/{MAX_NUMBER}";
            QuestUpdated();

            if (currentNumber >= MAX_NUMBER)
            {
                QuestFinished();
            }
        }
        asDoAnChargedAttack = false;
    }
}
