using UnityEngine;
public class MonsterHeart : ItemEffect, IPassiveItem
{
    private int maxLifeStat = 10;
    private int corruptionStat = 2;

    public void OnRemove()
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.DecreaseValue(Stat.MAX_HP, maxLifeStat, false);
        player.Stats.DecreaseValue(Stat.HP, maxLifeStat, false);
        player.Stats.DecreaseValue(Stat.CORRUPTION, corruptionStat, false);
    }

    public void OnRetrieved()  
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.IncreaseValue(Stat.MAX_HP, maxLifeStat, false);
        player.Stats.IncreaseValue(Stat.HP, maxLifeStat, false);
        player.Stats.IncreaseValue(Stat.CORRUPTION, corruptionStat, false);
        //RarityTier = Rarity.RARE;
        //Name = "<color=\"blue\">Monster's heart";
        //Description = "Boosts max health while corrupting the player, a perilous choice between vitality and darkness.\n" +
        //    "<color=\"green\">Max health: +" + maxLifeStat.ToString() + "  <color=\"purple\">Corruption: +" + corruptionStat.ToString();
    }
}
