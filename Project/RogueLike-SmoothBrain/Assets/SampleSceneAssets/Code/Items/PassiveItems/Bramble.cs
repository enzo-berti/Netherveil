using System;
using UnityEngine;
[Serializable]
public class Bramble : ItemEffect, IPassiveItem
{
    private int attackStat = 2;

    public void OnRemove()
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.DecreaseValue(Stat.ATK, attackStat, false);
    }

    public override void OnRetrieved()
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.IncreaseValue(Stat.ATK, attackStat, false);
        // Description = "Amplifies player's attack, infusing their strikes with the thorny power of the wild.\n <color=\"green\">Attack: +" + attackStat.ToString();
    }
}
