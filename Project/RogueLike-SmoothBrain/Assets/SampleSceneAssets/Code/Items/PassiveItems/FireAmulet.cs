using UnityEngine; 
 
public class FireAmulet : ItemEffect , IPassiveItem 
{
    private readonly float fireChance = 0.3f;
    private readonly float fireDuration = 2.0f;
    private readonly float fireFrequency = 0.5f;
    int indexInStatus = 0;
    public void OnRetrieved() 
    {
        Hero hero = GameObject.FindWithTag("Player").GetComponent<Hero>();
        indexInStatus = hero.StatusToApply.Count;
        hero.StatusToApply.Add(new Fire(fireDuration, fireChance, fireFrequency));
    } 
 
    public void OnRemove() 
    {
        GameObject.FindWithTag("Player").GetComponent<Hero>().StatusToApply.RemoveAt(indexInStatus);
    } 
 
} 
