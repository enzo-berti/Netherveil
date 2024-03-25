using UnityEngine;
public class TomeOfBenediction : ItemEffect, IPassiveItem
{
    private int increaseBenediction = 15;
    public void OnRemove()
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.IncreaseValue(Stat.CORRUPTION, increaseBenediction, false);
    }
    public void OnRetrieved()
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.DecreaseValue(Stat.CORRUPTION, increaseBenediction, false);
    }
}