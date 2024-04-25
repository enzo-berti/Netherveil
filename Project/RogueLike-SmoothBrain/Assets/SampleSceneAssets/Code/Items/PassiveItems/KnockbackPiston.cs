public class KnockbackPiston : ItemEffect , IPassiveItem 
{ 
    public void OnRetrieved() 
    {
        Hero.OnBasicAttack += Utilities.Hero.ApplyKnockback;
        Hero.OnDashAttack += Utilities.Hero.ApplyKnockback;
    } 
 
    public void OnRemove() 
    {
        Hero.OnBasicAttack -= Utilities.Hero.ApplyKnockback;
        Hero.OnDashAttack -= Utilities.Hero.ApplyKnockback;
    } 
 
} 
