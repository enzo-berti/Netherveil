using UnityEngine;

public class RuneOfWrath : ItemData, IPassiveItem
{
    private int AttackCoeffStat = 2;

    public void OnRetrieved()
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.SetValue(Stat.ATK_COEFF, AttackCoeffStat);
        RarityTier = Rarity.LEGENDARY;
        Name = "<color=\"yellow\">Rune of Wrath";
        Description = "Elevates player's attack coefficient, unleashing a surge of furious power in every strike.\n" +
            "<color=\"green\">Attack coefficient: +" + AttackCoeffStat.ToString();
    }
}
