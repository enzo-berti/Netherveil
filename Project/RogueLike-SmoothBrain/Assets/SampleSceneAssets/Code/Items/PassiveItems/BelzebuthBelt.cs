using UnityEngine;

public class BelzebuthBelt : ItemEffect, IPassiveItem
{
    private int critRateStat = 25;
    private int corruptionStat = 2;

    public void OnRemove()
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.DecreaseValue(Stat.CRIT_RATE, critRateStat, false);
        player.Stats.DecreaseValue(Stat.CORRUPTION, corruptionStat, false);
    }

    public void OnRetrieved()
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.IncreaseValue(Stat.CRIT_RATE, critRateStat, false);
        player.Stats.IncreaseValue(Stat.CORRUPTION, corruptionStat, false);
    }
}
