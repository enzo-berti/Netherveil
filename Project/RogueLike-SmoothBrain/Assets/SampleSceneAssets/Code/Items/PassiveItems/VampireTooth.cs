using Unity.VisualScripting;
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
        player.OnHit += LifeSteal;
        //RarityTier = Rarity.RARE;
        //Name = "<color=\"blue\">Vampire Tooth";
        //Description = "Provides lifesteal, allowing the player to recover a percentage of inflicted damage as health with each successful strike.\n" +
        //    "<color=\"green\">Life Steal: +" + lifeStealPourcentage.ToString() + "%";
    }

    public void LifeSteal(IDamageable damageable)
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        //life steal is a pourcentage that's incresed by items
        int lifeIncreasedValue = (int)(player.Stats.GetValueStat(Stat.LIFE_STEAL) * (player.Stats.GetValueStat(Stat.ATK) * player.Stats.GetValueStat(Stat.ATK_COEFF)));
        player.Stats.IncreaseValue(Stat.HP, lifeIncreasedValue);
        if (player.Stats.GetValueStat(Stat.HP) > player.Stats.GetValueStat(Stat.MAX_HP))
        {
            player.Stats.SetValue(Stat.HP, player.Stats.GetValueStat(Stat.MAX_HP));
        }
    }
}
