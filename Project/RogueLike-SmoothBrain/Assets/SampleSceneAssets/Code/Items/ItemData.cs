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
        ACTIVE,
        PASSIVE_ACTIVE
    } 

    public Rarity RarityTier;
    public ItemType Type;
    public string idName;
    public int price;
    public Material mat;
    public Mesh mesh;
    [Multiline] public string Description;
    public Texture icon;
}   
