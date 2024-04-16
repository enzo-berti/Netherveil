using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ItemEffect
{
    public string Name { get; set; } = string.Empty;
    public bool HasBeenRetreived { get; set; } = false;
    public float CurrentEnergy = 0f;
}