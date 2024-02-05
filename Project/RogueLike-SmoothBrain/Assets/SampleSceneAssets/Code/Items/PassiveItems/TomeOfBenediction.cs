using UnityEngine; 
 
public class TomeOfBenediction : ItemEffect , IPassiveItem 
{ 
    public void OnRemove()
    {
        throw new System.NotImplementedException();
    }
    public override void OnRetrieved() 
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.DecreaseValue(Stat.CORRUPTION, 15);
    } 
} 
