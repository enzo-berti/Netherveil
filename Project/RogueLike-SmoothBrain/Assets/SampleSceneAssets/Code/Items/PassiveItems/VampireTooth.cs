using Unity.VisualScripting;
using UnityEngine;

public class VampireTooth : ItemEffect, IPassiveItem
{
    //between 0 and 1
    const float lifeStealStat = 0.2f;
    public void OnRetrieved()
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.IncreaseValue(Stat.LIFE_STEAL, lifeStealStat);
        player.OnHit += LifeSteal;
        //RarityTier = Rarity.RARE;
        //Name = "<color=\"blue\">Vampire Tooth";
        //Description = "Provides lifesteal, allowing the player to recover a percentage of inflicted damage as health with each successful strike.\n" +
        //    "<color=\"green\">Life Steal: +" + lifeStealPourcentage.ToString() + "%";
    }

    public void LifeSteal(IDamageable damageable, IAttacker attacker)
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        //life steal is a pourcentage that's incresed by items
        AudioManager.Instance.PlaySound(player.GetComponent<PlayerController>().HealSFX, player.transform.position);
        int lifeIncreasedValue = (int)(player.Stats.GetValue(Stat.LIFE_STEAL) * player.Stats.GetValue(Stat.ATK));
        lifeIncreasedValue = (int)(lifeIncreasedValue * player.Stats.GetValue(Stat.HEAL_COEFF));
        FloatingTextGenerator.CreateHealText(lifeIncreasedValue, player.transform.position);
        player.Stats.IncreaseValue(Stat.HP, lifeIncreasedValue);
    }

    public void OnRemove()
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.DecreaseValue(Stat.LIFE_STEAL, lifeStealStat);
        player.OnHit -= LifeSteal;
    }
}
