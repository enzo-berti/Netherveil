using UnityEngine; 
 
public class BloodLover : ItemEffect , IPassiveItem 
{
    readonly float thresholdValue = 0.25f;
    readonly float coefValue = 0.5f;
    bool alreadyApplied = false;

#pragma warning disable CS0414 // Supprimer le warning dans unity
#pragma warning disable IDE0052 // Supprimer les membres privés non lus
    //used to display in description, dont delete it
    readonly float thresholdDisplayValue;
    readonly float coefDisplayValue;
#pragma warning restore IDE0052
#pragma warning restore CS0414

    public BloodLover()
    {
        thresholdDisplayValue = thresholdValue * 100f;
        coefDisplayValue = coefValue * 100f;
    }

    public void OnRetrieved() 
    {
        Hero.OnTakeDamage += Effect;
    }
 
    public void OnRemove() 
    {
        Hero.OnTakeDamage -= Effect;
    } 
    
    private void Effect(int damages, IAttacker attacker)
    {
        Hero hero = GameObject.FindWithTag("Player").GetComponent<Hero>();
        if(hero.Stats.GetValue(Stat.HP)/ hero.Stats.GetMaxValue(Stat.HP) <= thresholdValue && !alreadyApplied)
        {
            hero.Stats.IncreaseCoeffValue(Stat.ATK, coefValue);
            alreadyApplied = true;
        }
        else
        {
            alreadyApplied = false;
        }
    }
} 
