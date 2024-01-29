using UnityEngine;

public class MonsterHeart : Item, IPassiveItem
{
    private int maxLifeStat = 10;
    private int corruptionStat = 10;

    public void OnRetrieved()
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.SetValue(Stat.MAX_HP, maxLifeStat);
        player.Stats.SetValue(Stat.CORRUPTION, corruptionStat);
        RarityTier = Rarity.RARE;
        Name = "Monster's heart";
        Description = "Boosts max health while corrupting the player, a perilous choice between vitality and darkness.\n" +
            "<color=\"green\">Max health: +" + maxLifeStat.ToString() + "  <color=\"purple\">Corruption: +" + corruptionStat.ToString();
    }
}
