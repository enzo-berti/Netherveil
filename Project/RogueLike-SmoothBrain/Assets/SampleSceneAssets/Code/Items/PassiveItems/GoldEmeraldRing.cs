using UnityEngine;

public class GoldEmeraldRing : ItemEffect, IPassiveItem
{
    private int attackStat = 2;
    private int healthIncrease = 10;
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
        //RarityTier = Rarity.RARE;
        //Name = "<color=\"blue\">Gold Emerald Ring";
        //Description = "Augments player's attack, gilding their strikes with the radiant power of the emerald, enhancing combat effectiveness.\n" +
        //    "<color=\"green\">Attack: +" + attackStat.ToString();
    }
}
