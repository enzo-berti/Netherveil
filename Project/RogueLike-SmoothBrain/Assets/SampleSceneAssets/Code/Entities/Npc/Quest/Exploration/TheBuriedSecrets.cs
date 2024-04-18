using UnityEngine; 
 
public class TheBuriedSecrets : Quest 
{
    int currentNumber = 0;
    readonly int MAX_NUMBER = 15;

    public override void AcceptQuest()
    {
        progressText = $"NB SECRETS ROOM DISCOVERED : {currentNumber}/{MAX_NUMBER}";
        Hero.OnChargedAttack += UpdateCount;
    }

    protected override void QuestFinished()
    {
        base.QuestFinished();
        Hero.OnChargedAttack -= UpdateCount;
    }

    private void UpdateCount(IDamageable damageable, IAttacker attacker)
    {
        currentNumber++;
        progressText = $"NB SECRETS ROOM DISCOVERED : {currentNumber}/{MAX_NUMBER}";
        QuestUpdated();

        if (currentNumber >= MAX_NUMBER)
        {
            QuestFinished();
        }
    }


} 
