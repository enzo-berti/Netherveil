using UnityEngine; 
 
public class TomeOfBenediction : ItemEffect , IPassiveItem 
{ 
    public void OnRemove()
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.IncreaseValue(Stat.CORRUPTION, 15, false);
    }
    public override void OnRetrieved() 
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.DecreaseValue(Stat.CORRUPTION, 15, false);
    } 
} 
