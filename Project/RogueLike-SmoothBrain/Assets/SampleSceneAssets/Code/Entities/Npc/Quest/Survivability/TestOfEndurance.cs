using Map;

public class TestOfEndurance : Quest
{
    int currentSurvivedRoom = 0;
    int NB_ROOM_SURVIVING;

    public override void AcceptQuest()
    {
        
        switch (difficulty)
        {
            case QuestDifficulty.EASY:
                NB_ROOM_SURVIVING = 2;
                break;
            case QuestDifficulty.MEDIUM:
                Datas.CorruptionModifierValue += 5;
                NB_ROOM_SURVIVING = 4;
                break;
            case QuestDifficulty.HARD:
                Datas.CorruptionModifierValue += 10;
                NB_ROOM_SURVIVING = 6;
                break;
        }
        progressText = $"DON'T FALL UNDER 100HP DURING {NB_ROOM_SURVIVING} FIGHTS : {currentSurvivedRoom}/{NB_ROOM_SURVIVING}";
        RoomUtilities.allEnemiesDeadEvents += UpdateCount;
        Hero.OnTakeDamage += TestHp;
    }

    protected override void QuestLost()
    {
        base.QuestLost();
        Hero.OnTakeDamage -= TestHp;
    }

    protected override void QuestFinished()
    {
        base.QuestFinished();
        RoomUtilities.allEnemiesDeadEvents -= UpdateCount;
    }

    private void TestHp(int _arg, IAttacker _attacker)
    {
        if (Utilities.Hero.Stats.GetValue(Stat.HP) < 100)
        {
            QuestLost();
        }
    }

    private void UpdateCount()
    {
        currentSurvivedRoom++;
        progressText = $"DON'T FALL UNDER 100HP DURING {NB_ROOM_SURVIVING} FIGHTS : {currentSurvivedRoom}/{NB_ROOM_SURVIVING}";
        QuestUpdated();

        if (currentSurvivedRoom >= NB_ROOM_SURVIVING)
        {
            QuestFinished();
        }
    }
}