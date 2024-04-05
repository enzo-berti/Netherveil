using UnityEngine;

public class RuneOfPride : ItemEffect, IPassiveItem
{
    [SerializeField] private int maxBoost = 5;
    [SerializeField] private int boostValue = 5;
    private readonly int MAX_BOOST;
    private int nbBoost = 0;
    
    public RuneOfPride()
    {
        MAX_BOOST = maxBoost * boostValue;
    }

    public void OnRemove()
    {
        Hero.OnKill -= ctx => Berserk();
        RoomUtilities.EnterEvents -= Reset;
    }

    public void OnRetrieved() 
    { 
        Hero player = GameObject.FindWithTag("Player").GetComponent<Hero>();
        Hero.OnKill += ctx => Berserk();
        RoomUtilities.EnterEvents += Reset;
    } 

    private void Berserk()
    {
        
        Hero player = GameObject.FindWithTag("Player").GetComponent<Hero>();
        if (nbBoost * boostValue > MAX_BOOST) return;
        player.Stats.IncreaseValue(Stat.ATK, boostValue, false);
        player.Stats.IncreaseValue(Stat.ATK_RANGE, boostValue, false);
        nbBoost++;
    }

    private void Reset()
    {
        Hero player = GameObject.FindWithTag("Player").GetComponent<Hero>();
        player.Stats.DecreaseValue(Stat.ATK, boostValue * nbBoost, false);
        player.Stats.DecreaseValue(Stat.ATK_RANGE, boostValue * nbBoost, false);
        nbBoost = 0;
    }
} 
