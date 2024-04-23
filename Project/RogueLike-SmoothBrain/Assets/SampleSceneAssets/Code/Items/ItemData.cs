using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemData : IComparable<ItemData>
{
    public static readonly List<float> rarityWeighting = new List<float>()
    {
        0.2f,
        0.4f,
        0.25f,
        0.1f,
        0.05f
    };
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

    public int CompareTo(ItemData other)
    {
        return this.idName.CompareTo(other.idName);
    }
}   
