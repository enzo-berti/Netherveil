using UnityEngine;

public class GoldEmeraldRing : ItemEffect, IPassiveItem
{
    private readonly int attackStat = 2;
    private readonly int healthIncrease = 10;
    public void OnRemove()
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.DecreaseValue(Stat.ATK, attackStat, false);
        player.Stats.DecreaseMaxValue(Stat.HP, healthIncrease);
    }

    public void OnRetrieved()
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.IncreaseValue(Stat.ATK, attackStat, false);
        player.Stats.DecreaseValue(Stat.HP, healthIncrease, false);
    }
}
