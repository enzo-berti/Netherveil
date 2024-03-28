using UnityEngine;

public class SilverAmethystRing : ItemEffect, IPassiveItem
{
    private int maxLifeStat = 10;
    private int speed = 1;

    public void OnRemove()
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.DecreaseMaxValue(Stat.HP, maxLifeStat);
        player.Stats.DecreaseValue(Stat.HP, maxLifeStat, true);
        player.Stats.DecreaseValue(Stat.SPEED, speed, false);
    }

    public void OnRetrieved()
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.IncreaseMaxValue(Stat.HP, maxLifeStat);
        player.Stats.IncreaseValue(Stat.HP, maxLifeStat, true);
        player.Stats.IncreaseValue(Stat.SPEED, speed, false);
        //RarityTier = Rarity.RARE;
        //Name = "<color=\"blue\">Silver Amethyst Ring";
        //Description = "Elevates player's max health and agility simultaneously, infusing them with both vitality and swiftness for a well-rounded advantage.\n" +
        //    "<color=\"green\">Max health: +" + maxLifeStat.ToString() + "  Speed: +" + speed.ToString();

    }
}
