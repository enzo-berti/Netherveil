using Map;
using System;
using UnityEngine;

public class TestOfEndurance : Quest
{
    int currentSurvivedRoom = 0;
    int NB_ROOM_SURVIVING;

    public override void AcceptQuest()
    {
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