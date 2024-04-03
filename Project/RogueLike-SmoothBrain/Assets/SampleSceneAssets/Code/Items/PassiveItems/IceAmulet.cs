using UnityEngine; 
 
public class IceAmulet : ItemEffect , IPassiveItem 
{
    private float iceChance = 0.1f;
    private float iceDuration = 2.0f;
    int indexInStatus = 0;
    public void OnRetrieved()
    {
        Hero hero = GameObject.FindWithTag("Player").GetComponent<Hero>();
        indexInStatus = hero.StatusToApply.Count;
        hero.StatusToApply.Add(new Freeze(iceDuration, iceChance));
    }

    public void OnRemove()
    {
        GameObject.FindWithTag("Player").GetComponent<Hero>().StatusToApply.RemoveAt(indexInStatus);
    }

} 
