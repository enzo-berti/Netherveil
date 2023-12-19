using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Drop
{
    [SerializeField] List<DropInfo> dropList = new();
    public void DropLoot(Vector3 position)
    {
        foreach (DropInfo dropInfo in dropList)
        {
            if (dropInfo.isChanceShared)
            {
                if (UnityEngine.Random.value <= dropInfo.chance)
                {
                    for (int i = 0; i < dropInfo.Quantity; i++)
                    {
                        GameObject.Instantiate(dropInfo.loot, position, Quaternion.identity);
                    }
                }
            }
            else
            {
                for (int i = 0; i < dropInfo.Quantity; i++)
                {
                    if (UnityEngine.Random.value <= dropInfo.chance)
                    {
                        GameObject.Instantiate(dropInfo.loot, position, Quaternion.identity);
                    }
                }
            }

        }
    }
}
