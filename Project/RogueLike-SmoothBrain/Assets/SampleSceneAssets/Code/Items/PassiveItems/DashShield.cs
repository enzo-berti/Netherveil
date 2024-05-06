using UnityEngine; 
 
public class DashShield : ItemEffect , IPassiveItem 
{
    public void OnRetrieved() 
    {
        Utilities.PlayerInput.OnStartDash += ApplyShield;
        Utilities.PlayerInput.OnEndDash += RemoveShield;
        Utilities.PlayerInput.OnEndDashAttack += RemoveShield;
    } 
 
    public void OnRemove() 
    {
        Utilities.PlayerInput.OnStartDash -= ApplyShield;
        Utilities.PlayerInput.OnEndDash -= RemoveShield;
        Utilities.PlayerInput.OnEndDashAttack -= RemoveShield;
    } 
 
    private void ApplyShield()
    {
        //enable Shield VFX
        GameObject.FindWithTag("Player").GetComponent<Hero>().IsInvincibleCount++;
        Utilities.Player.GetComponent<PlayerController>().DashShieldVFX.Reinit();
        Utilities.Player.GetComponent<PlayerController>().DashShieldVFX.Play();
    }

    private void RemoveShield(Vector3 playerPos)
    {
        //disable Shield VFX
        GameObject.FindWithTag("Player").GetComponent<Hero>().IsInvincibleCount--;
        Utilities.Player.GetComponent<PlayerController>().DashShieldVFX.Stop();
    }
} 
