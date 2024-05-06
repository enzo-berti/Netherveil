using UnityEngine; 
 
public class UrnOfAvarice : ItemEffect , IPassiveItem 
{
    readonly float bloodDamagesCoef = 0.01f;

    public void OnRetrieved() 
    {
        Utilities.Hero.OnBeforeApplyDamages += AddBloodDamages;
    } 
 
    public void OnRemove() 
    {
        Utilities.Hero.OnBeforeApplyDamages -= AddBloodDamages;
    }

    private void AddBloodDamages(ref int damages, IDamageable target)
    {
        damages += (int)(Utilities.Hero.Inventory.Blood.Value * (bloodDamagesCoef));
    }
} 
