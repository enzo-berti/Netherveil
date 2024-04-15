using UnityEngine;

public class RuneOfWrath : ItemEffect, IPassiveItem
{
    private int AttackCoeffStat = 1;

    public void OnRemove()
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.DecreaseCoeffValue(Stat.ATK, AttackCoeffStat);
    }

    public void OnRetrieved()
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.IncreaseCoeffValue(Stat.ATK, AttackCoeffStat);
    }
}
