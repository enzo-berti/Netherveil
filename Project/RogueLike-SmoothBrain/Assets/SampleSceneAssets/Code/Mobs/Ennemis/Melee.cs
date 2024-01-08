using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : Sbire
{
    void Update()
    {
        foreach (var stat in stats.StatsName)
        {
            Debug.Log(stat);
        }
    }
}
