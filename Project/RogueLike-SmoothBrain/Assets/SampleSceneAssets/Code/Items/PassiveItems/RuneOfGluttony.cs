using UnityEngine; 
 
public class RuneOfGluttony : ItemEffect , IPassiveItem 
{ 
    public override void OnRetrieved() 
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.MultiplyValue(Stat.HEAL_COEFF, 2f);
    } 
} 
