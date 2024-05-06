using Map;

public class TestOfEndurance : Quest
{
    int currentSurvivedRoom = 0;
    int NB_ROOM_SURVIVING;

    public override void AcceptQuest()
    {
        base.AcceptQuest();
        switch (difficulty)
        {
            case QuestDifficulty.EASY:
                NB_ROOM_SURVIVING = 2;
                break;
            case QuestDifficulty.MEDIUM:
                CorruptionModifierValue += 5;
                NB_ROOM_SURVIVING = 4;
                break;
            case QuestDifficulty.HARD:
                CorruptionModifierValue += 10;
                NB_ROOM_SURVIVING = 6;
                break;
        }
        progressText = $"DON'T FALL UNDER 25% HP DURING {NB_ROOM_SURVIVING} FIGHTS : {currentSurvivedRoom}/{NB_ROOM_SURVIVING}";
        MapUtilities.onAllEnemiesDead += UpdateCount;
        Hero.OnTakeDamage += TestHp;
    }

    protected override void QuestLost()
    {
        base.QuestLost();
        Hero.OnTakeDamage -= TestHp;
        MapUtilities.onAllEnemiesDead -= UpdateCount;
    }

    protected override void QuestFinished()
    {
        base.QuestFinished();
        Hero.OnTakeDamage -= TestHp;
        MapUtilities.onAllEnemiesDead -= UpdateCount;
    }

    private void TestHp(int _arg, IAttacker _attacker)
    {
        if (IsQuestLost())
        {
            questLost = true;
        }
    }

    private void UpdateCount()
    {
        if (!IsQuestFinished() && !questLost)
        {
            currentSurvivedRoom++;
            progressText = $"DON'T FALL UNDER 25% HP DURING {NB_ROOM_SURVIVING} FIGHTS : {currentSurvivedRoom}/{NB_ROOM_SURVIVING}";
        }
        QuestUpdated();
    }

    protected override bool IsQuestFinished()
    {
        return currentSurvivedRoom >= NB_ROOM_SURVIVING;
    }

    protected bool IsQuestLost()
    {
        return Utilities.Hero.Stats.GetValue(Stat.HP) / Utilities.Hero.Stats.GetMaxValue(Stat.HP) < 0.25f;
    }
}