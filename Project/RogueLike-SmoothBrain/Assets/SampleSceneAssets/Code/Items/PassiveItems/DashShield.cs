using UnityEngine; 
 
public class DashShield : ItemEffect , IPassiveItem 
{
    public void OnRetrieved() 
    {
        PlayerInput.OnStartDash += ApplyShield;
        PlayerInput.OnEndDash += RemoveShield;
    } 
 
    public void OnRemove() 
    {
        PlayerInput.OnStartDash -= ApplyShield;
        PlayerInput.OnEndDash -= RemoveShield;
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
