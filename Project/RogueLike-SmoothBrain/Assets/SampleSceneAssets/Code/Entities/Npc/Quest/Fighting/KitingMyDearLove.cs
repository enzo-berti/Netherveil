using UnityEngine; 
 
public class KitingMyDearLove : Quest 
{
    int currentNumber = 0;
    bool asDoAnDistanceAttack = false;
    readonly int MAX_NUMBER = 10;

    public override void AcceptQuest()
    {
        progressText = $"NB ENEMIES KILL WITH DISTANCE ATTACK : {currentNumber}/{MAX_NUMBER}";
        Hero.OnSpearAttack += SetBool;
        Hero.OnKill += UpdateCount;
    }

    protected override void QuestFinished()
    {
        base.QuestFinished();
        Hero.OnSpearAttack -= SetBool;
        Hero.OnKill -= UpdateCount;
    }

    private void SetBool(IDamageable damageable, IAttacker attacker)
    {
        asDoAnDistanceAttack = true;
    }

    private void UpdateCount(IDamageable damageable)
    {
        Entity monster = (damageable as Entity);
        if (asDoAnDistanceAttack && monster != null && monster.Stats.GetValue(Stat.HP) <= 0)
        {
            currentNumber++;
            progressText = $"NB ENEMIES KILL WITH DISTANCE ATTACK : {currentNumber}/{MAX_NUMBER}";
            QuestUpdated();

            if (currentNumber >= MAX_NUMBER)
            {
                QuestFinished();
            }
        }
        asDoAnDistanceAttack = false;
    }


} 
