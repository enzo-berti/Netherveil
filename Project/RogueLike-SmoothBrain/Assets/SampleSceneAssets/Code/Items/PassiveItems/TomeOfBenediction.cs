using UnityEngine; 
 
public class TomeOfBenediction : ItemEffect , IPassiveItem 
{ 
    public override void OnRetrieved() 
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.DecreaseValue(Stat.CORRUPTION, 15);
    } 
} 
