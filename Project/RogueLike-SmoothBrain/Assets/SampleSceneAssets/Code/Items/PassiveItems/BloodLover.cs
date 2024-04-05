using UnityEngine; 
 
public class BloodLover : ItemEffect , IPassiveItem 
{
    readonly float thresholdValue = 0.25f;
    readonly float coefValue = 0.5f;
    bool alreadyApplied = false;
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
