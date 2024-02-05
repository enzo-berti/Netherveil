using UnityEngine;

public class DragonScale : ItemEffect, IPassiveItem
{
    //between 0 and 1
    const float critRateStat = .10f;
    //pourcentage between 0 and 100 or more
    const int critRateStatPourcentage = (int)(critRateStat * 100f);

    public void OnRemove()
    {
        throw new System.NotImplementedException();
    }

    public override void OnRetrieved()
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.IncreaseValue(Stat.CRIT_RATE, critRateStat);
        //RarityTier = Rarity.RARE;
        //Name = "<color=\"blue\">Dragon Scale";
        //Description = "Boosts player's critical hit rate, infusing their attacks with the precision of dragonkind.\n" +
        //    "<color=\"green\">Critical hit rate: +" + critRateStatPourcentage.ToString() + "%";
    }
}
