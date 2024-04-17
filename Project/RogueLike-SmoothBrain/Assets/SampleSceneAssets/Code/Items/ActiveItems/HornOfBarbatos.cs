using Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HornOfBarbatos : ItemEffect, IActiveItem
{
    public float Cooldown { get; set; } = 10f;
    private float increaseValue = 0.2f;
    private List<Stat> avoidedStat = new List<Stat>()
    {
        Stat.CORRUPTION,
        Stat.ATK_RANGE,
        Stat.HP
    };

    public void Activate()
    {
        RoomUtilities.ExitEvents += ResetStat;
        Hero hero = Utilities.Hero;
        foreach(var stat in hero.Stats.StatsName)
        {
            if(!avoidedStat.Contains(stat))
            {
                hero.Stats.MultiplyCoeffValue(stat, 1+increaseValue);
            }
        }
    }

    private void ResetStat()
    {
        Hero hero = Utilities.Hero;
        foreach (var stat in hero.Stats.StatsName)
        {
            if (!avoidedStat.Contains(stat))
            {
                hero.Stats.DivideCoeffValue(stat, 1+increaseValue);
            }
        }
    }
} 
