using UnityEngine; 
 
public class LockPickingKit : ItemEffect , IPassiveItem 
{ 
    public void OnRetrieved() 
    {
        GameObject.FindWithTag("Player").GetComponent<Hero>().TakingDamageFromTraps = false; 
    } 
 
    public void OnRemove() 
    {
        GameObject.FindWithTag("Player").GetComponent<Hero>().TakingDamageFromTraps = true;
    } 
 
} 
