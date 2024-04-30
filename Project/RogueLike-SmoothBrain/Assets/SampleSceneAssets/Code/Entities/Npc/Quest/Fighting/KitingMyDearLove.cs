
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
                break;
            case QuestDifficulty.MEDIUM:
                MAX_NUMBER = 8;
                Datas.CorruptionModifierValue += 5;
                break;
            case QuestDifficulty.HARD:
                MAX_NUMBER = 10;
                Datas.CorruptionModifierValue += 10;
                break;
        }
        progressText = $"NB ENEMIES KILL WITH DISTANCE ATTACK : {currentNumber}/{MAX_NUMBER}";
        Hero.OnSpearAttack += SetBool;
        Hero.OnKill += UpdateCount;
    }

    protected override bool IsQuestFinished()
    {
        return currentNumber >= MAX_NUMBER;
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
        if (!IsQuestFinished())
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
