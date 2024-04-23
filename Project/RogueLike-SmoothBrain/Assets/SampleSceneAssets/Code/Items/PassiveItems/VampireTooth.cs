using UnityEngine;

public class VampireTooth : ItemEffect, IPassiveItem
{
    const float lifeStealStat = 20f;
    public void OnRetrieved()
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.IncreaseValue(Stat.LIFE_STEAL, lifeStealStat/100f);
        player.OnAttackHit += LifeSteal;
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
        player.Stats.DecreaseValue(Stat.LIFE_STEAL, lifeStealStat / 100f);
        player.OnAttackHit -= LifeSteal;
    }
}
