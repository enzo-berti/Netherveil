using UnityEngine; 
 
public class Test : ItemEffect, IPassiveItem, IActiveItem 
{ 
    public void  OnRetrieved()
    {
    }
    public void  OnRemove()
    {
    }

    public System.Single Cooldown { get; set; }

    public void  OnActive()
    {
    }
    
 
} 
