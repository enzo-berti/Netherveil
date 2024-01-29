using UnityEngine;

public class BootOfSwiftness : Item, IPassiveItem
{
    private float speedStat = 1.5f;

    public void OnRetrieved()
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.IncreaseValue(Stat.SPEED, speedStat);
        RarityTier = Rarity.RARE;
        Name = "<color=\"blue\">Boots of Swiftness";
        Description = "Heightens player speed, granting swift agility to outpace challenges in the blink of an eye.\n" +
            "<color=\"green\">Speed: +" + speedStat.ToString();
    }
}
