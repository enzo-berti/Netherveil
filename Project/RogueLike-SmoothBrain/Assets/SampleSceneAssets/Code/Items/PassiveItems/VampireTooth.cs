using UnityEngine;

public class VampireTooth : ItemEffect, IPassiveItem
{
    const float lifeStealStat = 0.2f;
    public void OnRetrieved()
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.IncreaseValue(Stat.LIFE_STEAL, lifeStealStat/100f);
    }
    public void OnRemove()
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.DecreaseValue(Stat.LIFE_STEAL, lifeStealStat);
    }
}
