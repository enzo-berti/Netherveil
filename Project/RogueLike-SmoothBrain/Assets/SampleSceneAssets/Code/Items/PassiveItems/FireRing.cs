using UnityEngine; 
 
public class FireRing : ItemEffect , IPassiveItem 
{
    private float fireChance = 0.3f;
    private float fireDuration = 2.0f;
    int indexInStatus = 0;
    public void OnRetrieved() 
    {
        Hero hero = GameObject.FindWithTag("Player").GetComponent<Hero>();
        indexInStatus = hero.StatusToApply.Count;
        hero.StatusToApply.Add(new Fire(fireDuration, fireChance));
    } 
 
    public void OnRemove() 
    {
        GameObject.FindWithTag("Player").GetComponent<Hero>().StatusToApply.RemoveAt(indexInStatus);
    } 
 
} 
