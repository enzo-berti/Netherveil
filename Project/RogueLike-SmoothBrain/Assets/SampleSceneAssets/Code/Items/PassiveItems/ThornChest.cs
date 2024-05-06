using UnityEngine;

public class ThornChest : ItemEffect, IPassiveItem
{
    readonly float thornPourcentage = 15f;

    public void OnRetrieved()
    {
        Utilities.Hero.OnTakeDamage += ThornDamages;
    }

    public void OnRemove()
    {
        Utilities.Hero.OnTakeDamage -= ThornDamages;
    }

    private void ThornDamages(int damageValue, IAttacker attacker)
    {
        //check if attacker is different from null to prevent crash on trap damages
        GameObject vfx = GameObject.Instantiate(GameResources.Get<GameObject>("VFX_Thorn"));
        vfx.transform.position = (attacker as MonoBehaviour).transform.position + Vector3.up;
        vfx.GetComponent<VFXStopper>().Duration = 1f;
        vfx.GetComponent<VFXStopper>().PlayVFX();

        if (attacker != null && attacker is IDamageable)
        {
            (attacker as IDamageable).ApplyDamage((int)(damageValue * (thornPourcentage / 100f)), attacker);
        }
    }
}
