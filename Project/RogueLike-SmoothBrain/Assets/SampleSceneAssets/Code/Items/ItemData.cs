using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : ScriptableObject
{
    public enum Rarity
    {
        COMMON,
        UNCOMMON,
        RARE,
        EPIC,
        LEGENDARY
    }
    [SerializeField] ItemEffect script;


    public Rarity RarityTier;
    public string Name;
    public string Description;
}
