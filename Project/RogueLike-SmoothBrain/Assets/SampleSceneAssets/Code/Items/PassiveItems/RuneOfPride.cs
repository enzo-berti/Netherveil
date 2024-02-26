using UnityEngine;

public class RuneOfPride : ItemEffect, IPassiveItem
{
    [SerializeField] private int maxBoost = 5;
    [SerializeField] private int boostValue = 5;
    private int nbBoost = 0;
    
    public void OnRemove()
    {
        Hero player = GameObject.FindWithTag("Player").GetComponent<Hero>();
        player.OnKill -= ctx => Berserk();
        player.OnChangeRoom -= Reset;
    }

    public override void OnRetrieved() 
    { 
        Hero player = GameObject.FindWithTag("Player").GetComponent<Hero>();
        player.OnKill += ctx => Berserk();
        player.OnChangeRoom += Reset;
    } 

    private void Berserk()
    {
        
        Hero player = GameObject.FindWithTag("Player").GetComponent<Hero>();
        if (nbBoost > maxBoost) return;
        player.Stats.IncreaseValue(Stat.ATK, boostValue);
        player.Stats.IncreaseValue(Stat.ATK_RANGE, boostValue);
        nbBoost++;
    }

    private void Reset()
    {
        Hero player = GameObject.FindWithTag("Player").GetComponent<Hero>();
        player.Stats.DecreaseValue(Stat.ATK, boostValue * nbBoost);
        player.Stats.DecreaseValue(Stat.ATK_RANGE, boostValue * nbBoost);
        nbBoost = 0;
    }
} 
