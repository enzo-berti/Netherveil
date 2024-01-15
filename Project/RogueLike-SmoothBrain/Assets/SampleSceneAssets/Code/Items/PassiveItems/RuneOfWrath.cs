using UnityEngine;

public class RuneOfWrath : Item, IPassiveItem
{
    public void OnRetrieved()
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.SetValue(Stat.ATK_COEFF, 2);
    }
}
