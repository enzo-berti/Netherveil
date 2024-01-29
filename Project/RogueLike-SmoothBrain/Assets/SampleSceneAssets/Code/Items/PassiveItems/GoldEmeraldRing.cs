using UnityEngine;

public class GoldEmeraldRing : ItemEffect, IPassiveItem
{
    private int attackStat = 2;

    public override void OnRetrieved()
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.IncreaseValue(Stat.ATK, attackStat);
        //RarityTier = Rarity.RARE;
        //Name = "<color=\"blue\">Gold Emerald Ring";
        //Description = "Augments player's attack, gilding their strikes with the radiant power of the emerald, enhancing combat effectiveness.\n" +
        //    "<color=\"green\">Attack: +" + attackStat.ToString();
    }
}
