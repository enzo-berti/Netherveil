using UnityEngine;

public class SilverAmethystRing : Item , IPassiveItem
{
    private int maxLifeStat = 10;
    private int speed = 1;

    public void OnRetrieved()
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.SetValue(Stat.MAX_HP, maxLifeStat);
        player.Stats.IncreaseValue(Stat.HP, maxLifeStat);
        player.Stats.SetValue(Stat.SPEED, speed);
        RarityTier = Rarity.RARE;
        Name = "<color=\"blue\">Silver Amethyst Ring";
        Description = "Elevates player's max health and agility simultaneously, infusing them with both vitality and swiftness for a well-rounded advantage.\n" +
            "<color=\"green\">Max health: +" + maxLifeStat.ToString() + "  Speed: +" + speed.ToString();
    }
}
