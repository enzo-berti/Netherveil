using UnityEngine;

public class BootOfSwiftness : Item, IPassiveItem
{
    public void OnRetrieved()
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.IncreaseValue(Stat.SPEED, 1.5f);
    }
}
