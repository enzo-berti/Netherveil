using System.Collections;
using UnityEngine;

public class HornOfBarbatos : ItemEffect, IActiveItem
{
    public float Cooldown { get; set; } = 3f;

    public void Activate()
    {
        Debug.Log("WOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOH");
    }
} 
