using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(menuName = "Item")]
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

    public MonoScript effect;
    public Rarity RarityTier;
    public string Name;
    [Multiline] public string Description;
}
