using Map.Generation;
using System;
using System.Collections.Generic;
using System.Linq;

public class ItemPool
{
    #region Tier Chance
    private const float CommonChance = 0.2f;
    private const float UncommonChance = 0.4f;
    private const float RareChance = 0.25f;
    private const float EpicChance = 0.1f;
    private const float LegendaryChance = 0.05f;
    private const string DefaultItem = "MonsterHeart";
    private List<float> rarityWeighting = new List<float>()
    {
        CommonChance,
        UncommonChance,
        RareChance,
        EpicChance,
        LegendaryChance
    };
    #endregion

    #region Members
    List<List<string>> itemPerTier;
    #endregion

    #region Methods
    public ItemPool()
    {
        ItemDatabase itemDatabase = GameResources.Get<ItemDatabase>("ItemDatabase");
        itemPerTier = new List<List<string>>();
        for (int i = 0; i < Enum.GetNames(typeof(ItemData.Rarity)).Length; i++)
        {
            // Adding pool for each Rarity
            itemPerTier.Add(itemDatabase.datas.Where(x => Convert.ToInt32(x.RarityTier) == i).Select(x => x.idName).ToList());
        }

    }

    public bool IsPoolEmpty()
    {
        for(int i = 0; i < itemPerTier.Count; i++)
        {
            if (itemPerTier[i].Count > 0)
                return false;
        }
        return true;
    }

    public void UpdateRarityWeight(int indexEmpty)
    {
        float toShare = rarityWeighting[indexEmpty];
        rarityWeighting.RemoveAt(indexEmpty);
        toShare /= rarityWeighting.Count;
        for(int i = 0; i < rarityWeighting.Count;i++)
        {
            rarityWeighting[i] += toShare;
        }
    }

    public string GetRandomItemName()
    {
        if (IsPoolEmpty()) return DefaultItem;

        float randomRarity = Seed.Range();
        float currentChance = 0;
        int indexRarity = 0;
        for(int i = rarityWeighting.Count - 1; i >= 0  ; i--)
        {
            currentChance += rarityWeighting[i];
            if(randomRarity <= currentChance)
            {
                indexRarity = i;
                break;
            }
        }
        int randomItemIndex = Seed.Range(0, itemPerTier[indexRarity].Count);
        string toReturn =  itemPerTier[indexRarity][randomItemIndex];
        RemoveItemFromPool(indexRarity, toReturn);
        return toReturn;
    }

    private void RemoveItemFromPool(int rarityIndex, string name)
    {
        itemPerTier[rarityIndex].Remove(name);
        if (itemPerTier[rarityIndex].Count == 0)
        {
            UpdateRarityWeight(rarityIndex);
            itemPerTier.RemoveAt(rarityIndex);
        }
    }
    #endregion
}
