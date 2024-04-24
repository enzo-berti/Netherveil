using UnityEngine; 
 
public class UrnOfAvarice : ItemEffect , IPassiveItem 
{
    readonly float bloodDamagesCoef = 0.01f;

    public void OnRetrieved() 
    {
        Hero.OnBeforeApplyDamages += AddBloodDamages;
    } 
 
    public void OnRemove() 
    {
        Hero.OnBeforeApplyDamages -= AddBloodDamages;
    }

    private void AddBloodDamages(ref int damages, IDamageable target)
    {
        Hero hero = GameObject.FindWithTag("Player").GetComponent<Hero>();
        damages += (int)(hero.Inventory.Blood * (bloodDamagesCoef));
    }
} 
