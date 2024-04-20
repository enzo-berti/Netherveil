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
    public Color rarityColor
    {
        get
        {
            switch (RarityTier)
            {
                case Rarity.COMMON:         // Blanc
                    return Color.white;
                case Rarity.UNCOMMON:       // Bleu
                    return new Color(0.10f, 0.54f, 0.98f, 1.0f);
                case Rarity.RARE:           // Violet
                    return new Color(0.67f, 0.16f, 0.88f, 1.0f);
                case Rarity.EPIC:           // Orange
                    return new Color(0.87f, 0.38f, 0.08f, 1.0f);
                case Rarity.LEGENDARY:      // Jaune
                    return new Color(0.91f, 0.76f, 0.17f, 1.0f);
                default:
                    return Color.white;
            }
        }
    }

    public int CompareTo(ItemData other)
    {
        return this.idName.CompareTo(other.idName);
    }
}   
