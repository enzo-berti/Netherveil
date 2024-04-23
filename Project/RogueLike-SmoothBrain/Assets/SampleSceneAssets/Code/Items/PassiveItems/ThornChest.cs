using UnityEngine;

public class ThornChest : ItemEffect, IPassiveItem
{
    readonly float thornPourcentage = 15f;

    public void OnRetrieved()
    {
        Hero.OnTakeDamage += ThornDamages;
    }

    public void OnRemove()
    {
        Hero.OnTakeDamage -= ThornDamages;
    }

    private void ThornDamages(int damageValue, IAttacker attacker)
    {
        //check if attacker is different from null to prevent crash on trap damages
        if (attacker != null && attacker is IDamageable)
        {
            (attacker as IDamageable).ApplyDamage((int)(damageValue * (thornPourcentage / 100f)), attacker);
        }
    }
}
