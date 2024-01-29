using UnityEngine;

public class VampireTooth : ItemEffect, IPassiveItem
{
    //between 0 and 1
    const float lifeStealStat = .2f;
    //pourcentage between 0 and 100 or more
    const int lifeStealPourcentage = (int)(lifeStealStat * 100f);

    public override void OnRetrieved()
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.IncreaseValue(Stat.LIFE_STEAL, lifeStealStat);
        //RarityTier = Rarity.RARE;
        //Name = "<color=\"blue\">Vampire Tooth";
        //Description = "Provides lifesteal, allowing the player to recover a percentage of inflicted damage as health with each successful strike.\n" +
        //    "<color=\"green\">Life Steal: +" + lifeStealPourcentage.ToString() + "%";
    }
}
