using UnityEngine; 
 
public class RuneOfEnvy : ItemEffect , IPassiveItem 
{ 
    public void OnRetrieved() 
    { 
        // TODO : Lorsqu'on rentre dans une salle on absorbe une partie des stats de tout le monde
    } 
 
    public void OnRemove() 
    { 
        throw new System.NotImplementedException(); 
    } 
 
} 
