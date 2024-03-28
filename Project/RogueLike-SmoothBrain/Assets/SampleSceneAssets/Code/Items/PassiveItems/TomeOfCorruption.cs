using UnityEngine;

public class TomeOfCorruption : ItemEffect, IPassiveItem
{
    private int increaseCorruption = 15;
    public void OnRemove()
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.DecreaseValue(Stat.CORRUPTION, increaseCorruption, true);
    }

    public void OnRetrieved() 
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.IncreaseValue(Stat.CORRUPTION, increaseCorruption, true);
    } 
} 
