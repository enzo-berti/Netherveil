using System;
using UnityEngine;
[Serializable]
public class Bramble : ItemEffect, IPassiveItem
{
    private int attackStat = 2;

    public void OnRemove()
    {
        throw new NotImplementedException();
    }

    public override void OnRetrieved()
    {
        Hero player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        player.Stats.IncreaseValue(Stat.ATK, attackStat);
        // Description = "Amplifies player's attack, infusing their strikes with the thorny power of the wild.\n <color=\"green\">Attack: +" + attackStat.ToString();
    }
}
