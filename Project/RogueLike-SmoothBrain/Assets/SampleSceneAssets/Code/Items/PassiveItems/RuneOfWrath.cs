using UnityEngine;

public class RuneOfWrath : ItemEffect, IPassiveItem
{
    private int AttackCoeffStat = 1;

    public void OnRemove()
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.DecreaseCoeffValue(Stat.ATK, AttackCoeffStat);
    }

    public void OnRetrieved()
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.IncreaseCoeffValue(Stat.ATK, AttackCoeffStat);
        //RarityTier = Rarity.LEGENDARY;
        //Name = "<color=\"yellow\">Rune of Wrath";
        //Description = "Elevates player's attack coefficient, unleashing a surge of furious power in every strike.\n" +
        //    "<color=\"green\">Attack coefficient: +" + AttackCoeffStat.ToString();
    }
}
