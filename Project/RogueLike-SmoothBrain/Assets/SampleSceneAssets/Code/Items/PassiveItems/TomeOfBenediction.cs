using UnityEngine;

public class TomeOfBenediction : ItemEffect , IPassiveItem 
{
    readonly int value = 15;
    public void OnRetrieved() 
    {
        GameObject.FindWithTag("Player").GetComponent<Hero>().Stats.DecreaseValue(Stat.CORRUPTION, value);
    } 
 
    public void OnRemove() 
    {
        GameObject.FindWithTag("Player").GetComponent<Hero>().Stats.IncreaseValue(Stat.CORRUPTION, value);
    }
} 
