using UnityEngine; 
 
public class Test : ItemEffect, IActiveItem 
{ 
    public System.Single Cooldown { get; set; } = 10;

    public void  OnActive()
    {
    }
    
 
} 
