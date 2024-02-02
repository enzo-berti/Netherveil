using System;
using UnityEngine;
[Serializable]
public class ItemData
{
    public enum Rarity
    {
        COMMON,
        UNCOMMON,
        RARE,
        EPIC,
        LEGENDARY
    }
    public enum ItemType
    {
        PASSIVE,
        ACTIVE
    }
    public Rarity RarityTier;
    public ItemType Type;
    public string idName;
    [Multiline] public string Description;
}   
