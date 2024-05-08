using System.Collections.Generic;
using UnityEngine;
using Map;

public class RuneOfEnvy : ItemEffect, IPassiveItem
{
    readonly List<List<float>> statsStolen = new();
    readonly float stealPourcentage = 0.1f;

    enum StolenStats
    {
        HP,
        ATK,
        SPD,
        NB
    }

    public void OnRetrieved()
    {
        MapUtilities.onEnter += StealStats;
        MapUtilities.onExit += ResetStats;

        for(int i = 0; i< (int)StolenStats.NB; i++)
        {
            statsStolen.Add(new List<float>());
        }
    }

    public void OnRemove()
    {
        MapUtilities.onEnter -= StealStats;
        MapUtilities.onExit -= ResetStats;
    }

    private void StealStats()
    {
        Hero hero = GameObject.FindWithTag("Player").GetComponent<Hero>();

        if (MapUtilities.currentRoomData.enemies.Count > 0)
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.RuneOfEnvySFX);
        }

        foreach (GameObject enemy in MapUtilities.currentRoomData.enemies)
        {
            
            Mobs mob = enemy.GetComponent<Mobs>();
            mob.StatSuckerVFX.GetComponent<VFXStopper>().Duration = 1f;
            mob.StatSuckerVFX.GetComponent<VFXStopper>().PlayVFX();
            int hpStolen = (int)(mob.Stats.GetMaxValue(Stat.HP) * (stealPourcentage));
            float atkStolen = mob.Stats.GetValue(Stat.ATK) * (stealPourcentage);
            float speedStolen = mob.Stats.GetValue(Stat.SPEED) * (stealPourcentage);

            statsStolen[(int)StolenStats.HP].Add(hpStolen);
            statsStolen[(int)StolenStats.ATK].Add(atkStolen);
            statsStolen[(int)StolenStats.SPD].Add(speedStolen);

            hero.Stats.IncreaseMaxValue(Stat.HP, hpStolen);
            hero.Stats.IncreaseValue(Stat.HP, hpStolen);

            mob.Stats.DecreaseMaxValue(Stat.HP, hpStolen);
            mob.Stats.DecreaseValue(Stat.HP, hpStolen, false);

            hero.Stats.IncreaseValue(Stat.ATK, atkStolen);

            mob.Stats.DecreaseValue(Stat.ATK, atkStolen, false);

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
            hero.Stats.DecreaseValue(Stat.ATK, statStolen, false);
        }

        foreach (float statStolen in statsStolen[(int)StolenStats.SPD])
        {
            hero.Stats.DecreaseValue(Stat.SPEED, statStolen, false);
        }

        foreach(List<float> statsStolenList in statsStolen)
        {
            statsStolenList.Clear();
        }
    }
}
