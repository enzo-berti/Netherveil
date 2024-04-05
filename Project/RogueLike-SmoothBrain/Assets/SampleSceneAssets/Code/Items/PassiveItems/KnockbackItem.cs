
using UnityEngine;

public class KnockbackItem : ItemEffect , IPassiveItem 
{ 
    public void OnRetrieved() 
    {
        Hero.OnBasicAttack += GameObject.FindWithTag("Player").GetComponent<Hero>().ApplyKnockback;
    } 
 
    public void OnRemove() 
    {
        Hero.OnBasicAttack -= GameObject.FindWithTag("Player").GetComponent<Hero>().ApplyKnockback;
    } 
 
} 
