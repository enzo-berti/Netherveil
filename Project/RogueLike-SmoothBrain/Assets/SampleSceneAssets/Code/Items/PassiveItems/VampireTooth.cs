using Unity.VisualScripting;
using UnityEngine;

public class VampireTooth : ItemEffect, IPassiveItem
{
    //between 0 and 1
    const float lifeStealStat = .2f;
    //pourcentage between 0 and 100 or more
    const int lifeStealPourcentage = (int)(lifeStealStat * 100f);
    public void OnRetrieved()
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.IncreaseValue(Stat.LIFE_STEAL, lifeStealStat, false);
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
        float lifeIncreasedValue = (int)(player.Stats.GetValue(Stat.LIFE_STEAL) * (player.Stats.GetValue(Stat.ATK) * player.Stats.GetValue(Stat.ATK_COEFF)));
        lifeIncreasedValue = lifeIncreasedValue * player.Stats.GetValue(Stat.HEAL_COEFF);
        player.Stats.IncreaseValue(Stat.HP, lifeIncreasedValue, false);
        if (player.Stats.GetValue(Stat.HP) > player.Stats.GetValue(Stat.MAX_HP))
        {
            player.Stats.SetValue(Stat.HP, player.Stats.GetValue(Stat.MAX_HP));
        }
    }

    public void OnRemove()
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.IncreaseValue(Stat.LIFE_STEAL, lifeStealStat, false);
        player.OnHit -= LifeSteal;
    }
}
