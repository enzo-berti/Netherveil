using UnityEngine;
public class MonsterHeart : ItemEffect, IPassiveItem
{
    private int maxLifeStat = 10;
    private int corruptionStat = 2;

    public void OnRemove()
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.DecreaseMaxValue(Stat.HP, maxLifeStat);
        player.Stats.DecreaseValue(Stat.HP, maxLifeStat, true);
        player.Stats.DecreaseValue(Stat.CORRUPTION, corruptionStat, true);
    }

    public void OnRetrieved()  
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.IncreaseMaxValue(Stat.HP, maxLifeStat);
        player.Stats.IncreaseValue(Stat.HP, maxLifeStat, true);
        player.Stats.IncreaseValue(Stat.CORRUPTION, corruptionStat, true);
        //RarityTier = Rarity.RARE;
        //Name = "<color=\"blue\">Monster's heart";
        //Description = "Boosts max health while corrupting the player, a perilous choice between vitality and darkness.\n" +
        //    "<color=\"green\">Max health: +" + maxLifeStat.ToString() + "  <color=\"purple\">Corruption: +" + corruptionStat.ToString();
    }
}
