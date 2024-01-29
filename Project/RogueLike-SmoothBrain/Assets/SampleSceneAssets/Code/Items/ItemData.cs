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

    public TextAsset effect;
    public Rarity RarityTier;
    public string Name;
    [Multiline] public string Description;
}
