using UnityEngine;

public class DragonScale : Item, IPassiveItem
{
    private int critRateStat = 2;

    public void OnRetrieved()
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.IncreaseValue(Stat.CRIT_RATE, critRateStat);
        RarityTier = Rarity.RARE;
        Name = "<color=\"blue\">Dragon Scale";
        Description = "Boosts player's critical hit rate, infusing their attacks with the precision of dragonkind.\n" +
            "<color=\"green\">Critical hit rate: +" + critRateStat.ToString() + "%";
    }
}
