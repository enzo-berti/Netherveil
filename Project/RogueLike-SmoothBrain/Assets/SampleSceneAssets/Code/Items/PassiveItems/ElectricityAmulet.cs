using UnityEngine; 
 
public class ElectricityAmulet : ItemEffect , IPassiveItem 
{
    private readonly float electricityChance = 10f;
    private readonly float electricityDuration = 2.0f;
    int indexInStatus = 0;
    public void OnRetrieved()
    {
        Hero hero = GameObject.FindWithTag("Player").GetComponent<Hero>();
        indexInStatus = hero.StatusToApply.Count;
        hero.StatusToApply.Add(new Electricity(electricityDuration, electricityChance/100f));
    }

    public void OnRemove()
    {
        GameObject.FindWithTag("Player").GetComponent<Hero>().StatusToApply.RemoveAt(indexInStatus);
    }

} 
