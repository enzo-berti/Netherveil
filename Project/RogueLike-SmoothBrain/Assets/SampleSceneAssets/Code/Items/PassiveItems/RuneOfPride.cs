using UnityEngine;
using Map;
using UnityEngine.InputSystem;

public class RuneOfPride : ItemEffect, IPassiveItem
{
    [SerializeField] private int maxBoost = 4;
    [SerializeField] private int boostValue = 3;
    private readonly int MAX_BOOST;
    private int nbBoost = 0;
    
    public RuneOfPride()
    {
        MAX_BOOST = maxBoost * boostValue;
    }

    public void OnRemove()
    {
        Utilities.Hero.OnKill -= Berserk;
        MapUtilities.onEnter -= Reset;
    }

    public void OnRetrieved() 
    {
        Utilities.Hero.OnKill += Berserk;
        MapUtilities.onEnter += Reset;
    } 

    private void Berserk(IDamageable damageable)
    {
        Hero player = GameObject.FindWithTag("Player").GetComponent<Hero>();
        if (nbBoost * boostValue > MAX_BOOST) return;
        player.Stats.IncreaseValue(Stat.ATK, boostValue, false);
        nbBoost++;
        if(nbBoost == 1)
        {
            Utilities.Player.GetComponent<PlayerController>().RuneOfPrideVFX.Play();
        }
        Utilities.Player.GetComponent<PlayerController>().RuneOfPrideVFX.SetFloat("Arrows Amounts", 0.03f * nbBoost);
    }

    private void Reset()
    {
        Hero player = GameObject.FindWithTag("Player").GetComponent<Hero>();
        player.Stats.DecreaseValue(Stat.ATK, boostValue * nbBoost, false);
        nbBoost = 0;
        Utilities.Player.GetComponent<PlayerController>().RuneOfPrideVFX.Stop();
    }
} 
