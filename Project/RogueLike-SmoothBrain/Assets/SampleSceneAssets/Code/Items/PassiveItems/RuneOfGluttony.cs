using UnityEngine;

public class RuneOfGluttony : ItemEffect, IPassiveItem
{
    public void OnRemove()
    {
        throw new System.NotImplementedException();
    }

    public override void OnRetrieved() 
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.MultiplyValue(Stat.HEAL_COEFF, 2f);
    } 
} 
