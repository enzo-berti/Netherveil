using UnityEngine; 
 
public class SneakyDagger : ItemEffect , IPassiveItem 
{ 
    public void OnRetrieved() 
    { 
       
    } 
 
    public void OnRemove() 
    { 
        throw new System.NotImplementedException(); 
    } 
 

    private void ExtraSneakyDamages()
    {

    }
} 
