using UnityEngine;
public class Bramble : Item, IPassiveItem
{
    public void OnRetrieved()
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.IncreaseValue(Stat.ATK, 2);
    }
}
