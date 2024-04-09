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
        GameObject.FindWithTag("Player").GetComponent<Hero>().IsInvincible = true;
    }

    private void RemoveShield(Vector3 playerPos)
    {
        GameObject.FindWithTag("Player").GetComponent<Hero>().IsInvincible = false;
    }
} 
