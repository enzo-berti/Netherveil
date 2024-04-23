using System.Collections.Generic;
using UnityEngine;
using Map;

public class RuneOfEnvy : ItemEffect, IPassiveItem
{
    readonly List<List<float>> statsStolen = new();
    readonly float stealPourcentage = 10f;

    enum StolenStats
    {
        HP,
        ATK,
        SPD,
        NB
    }

    public void OnRetrieved()
    {
        RoomUtilities.enterEvents += StealStats;
        RoomUtilities.exitEvents += ResetStats;

        for(int i = 0; i< (int)StolenStats.NB; i++)
        {
            statsStolen.Add(new List<float>());
        }
    }

    public void OnRemove()
    {
        RoomUtilities.enterEvents -= StealStats;
        RoomUtilities.exitEvents -= ResetStats;
    }

    private void StealStats()
    {
        Hero hero = GameObject.FindWithTag("Player").GetComponent<Hero>();
        foreach (GameObject enemy in RoomUtilities.roomData.enemies)
        {
            Mobs mob = enemy.GetComponent<Mobs>();
            mob.StatSuckerVFX.GetComponent<VFXStopper>().Duration = 1f;
            mob.StatSuckerVFX.GetComponent<VFXStopper>().PlayVFX();
            float hpStolen = mob.Stats.GetValue(Stat.HP) * (stealPourcentage/100f);
            float atkStolen = mob.Stats.GetValue(Stat.ATK) * (stealPourcentage / 100f);
            float speedStolen = mob.Stats.GetValue(Stat.SPEED) * (stealPourcentage / 100f);

            statsStolen[(int)StolenStats.HP].Add(hpStolen);
            statsStolen[(int)StolenStats.ATK].Add(atkStolen);
            statsStolen[(int)StolenStats.SPD].Add(speedStolen);

            hero.Stats.IncreaseMaxValue(Stat.HP, hpStolen);
            hero.Stats.IncreaseValue(Stat.HP, hpStolen);
            mob.Stats.DecreaseValue(Stat.HP, hpStolen, false);

            hero.Stats.IncreaseMaxValue(Stat.ATK, atkStolen);
            hero.Stats.IncreaseValue(Stat.ATK, atkStolen);
            mob.Stats.DecreaseValue(Stat.ATK, atkStolen, false);

            hero.Stats.IncreaseMaxValue(Stat.SPEED, speedStolen);
            hero.Stats.IncreaseValue(Stat.SPEED, speedStolen);
            mob.Stats.DecreaseValue(Stat.SPEED, speedStolen, false);
        }
    }

    private void ResetStats()
    {
        Hero hero = GameObject.FindWithTag("Player").GetComponent<Hero>();

        foreach (float statStolen in statsStolen[(int)StolenStats.HP])
        {
            hero.Stats.DecreaseMaxValue(Stat.HP, statStolen);
            hero.Stats.DecreaseValue(Stat.HP, statStolen, false);
        }

        //protect player from dying when decreasing bonus HP
        if(hero.Stats.GetValue(Stat.HP) <= 0)
        {
            hero.Stats.SetValue(Stat.HP, 1);
        }

        foreach (float statStolen in statsStolen[(int)StolenStats.ATK])
        {
            hero.Stats.DecreaseMaxValue(Stat.ATK, statStolen);
            hero.Stats.DecreaseValue(Stat.ATK, statStolen, false);
        }

        foreach (float statStolen in statsStolen[(int)StolenStats.SPD])
        {
            hero.Stats.DecreaseMaxValue(Stat.SPEED, statStolen);
            hero.Stats.DecreaseValue(Stat.SPEED, statStolen, false);
        }

        foreach(List<float> statsStolenList in statsStolen)
        {
            statsStolenList.Clear();
        }
    }
}
