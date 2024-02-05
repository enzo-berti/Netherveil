using UnityEngine; 
 
public class TomeOfCorruption : ItemEffect , IPassiveItem 
{ 
    public override void OnRetrieved() 
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.IncreaseValue(Stat.CORRUPTION, 15);
    } 
} 
