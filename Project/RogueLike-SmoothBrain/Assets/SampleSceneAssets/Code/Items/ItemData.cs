using UnityEngine;

[CreateAssetMenu(fileName = "newItem", menuName = "Item/NewItem")]
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
    [SerializeField] Item script;
   

    public Rarity RarityTier;
    public string Name;
    public string Description;
}
