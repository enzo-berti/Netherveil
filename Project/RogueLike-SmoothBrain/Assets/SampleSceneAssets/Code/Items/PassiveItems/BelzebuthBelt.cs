using UnityEngine;

public class BelzebuthBelt : ItemEffect, IPassiveItem
{
    private int critRateStat = 25;
    private int corruptionStat = 2;

    public override void OnRetrieved()
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.IncreaseValue(Stat.CRIT_RATE, critRateStat);
        player.Stats.IncreaseValue(Stat.CORRUPTION, corruptionStat);
        //RarityTier = Rarity.RARE;
        //Name = "<color=\"blue\">Belzebuth's Belt";
        //Description = "Significantly raises critical hit rate while simultaneously increasing the player's corruption, tempting them with the dark allure of demonic power.\n" +
        //    "<color=\"green\">Critical hit rate: +" + critRateStat.ToString() + "  <color=\"purple\">Corruption: +" + corruptionStat.ToString();
    }
}
