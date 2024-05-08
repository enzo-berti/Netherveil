using Map.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.VFX;

// This class is the item that is rendered in the 3D world
[Serializable]
public class Item : MonoBehaviour
{
    public static event Action<ItemEffect> OnRetrieved;
    public static event Action OnLateRetrieved;
    public static float priceCoef = 1.0f;
    public static List<List<ItemData>> ItemPool;
    public static readonly int PRICE_PER_RARITY = 30;

    [SerializeField] private bool isRandomized = true;
    [SerializeField] private ItemDatabase database;
    [SerializeField] VisualEffect auraVFX;

    private ItemEffect itemEffect;
    private Color rarityColor = Color.white;
    public string idItemName = string.Empty;
    private int price;

    private ItemDescription itemDescription;
    public Color RarityColor => rarityColor;
    public ItemEffect ItemEffect => itemEffect;
    public ItemDatabase Database => database;

    public int Price => price;

    private void Awake()
    {
        if (ItemPool == null)
        {
            ItemPool = new List<List<ItemData>>();
            for (int i = 0; i < Enum.GetNames(typeof(ItemData.Rarity)).Length; i++)
            {
                // Adding pool for each Rarity
                ItemPool.Add(database.datas.Where(x => Convert.ToInt32(x.RarityTier) == i).ToList());
            }
        }

    }
    private void Start()
    {
        if (itemEffect == null)
        {
            if (isRandomized)
            {
                RandomizeItem(this);
            }
            CreateItem();
        }

    }
    public static void InvokeOnRetrieved(ItemEffect effect)
    {
        OnRetrieved?.Invoke(effect);
        OnLateRetrieved?.Invoke();
        AudioManager.Instance.PlaySound(AudioManager.Instance.PickUpItemSFX, Utilities.Player.transform.position);
    }

    private ItemEffect LoadClass()
    {
        return Assembly.GetExecutingAssembly().CreateInstance(idItemName.GetPascalCase()) as ItemEffect;
    }

    static public void RandomizeItem(Item item)
    {
        bool isEveryPoolEmpty = true;
        // If every pool empty then spawn MonsterHeart
        for (int i = 0; i < ItemPool.Count; i++)
        {
            if (ItemPool[i].Count > 0)
            {
                isEveryPoolEmpty = false;
                break;
            }
        }
        if (isEveryPoolEmpty)
        {
            item.idItemName = "MonsterHeart";
            return;
        }
        // List every item name in each pool ( Common, uncommon... )
        List<List<string>> allItemsByPool = new();
        for (int j = 0; j < ItemPool.Count; j++)
        {
            allItemsByPool.Add(new List<string>());
            for (int k = 0; k < ItemPool[j].Count; k++)
            {
                allItemsByPool[j].Add(ItemPool[j][k].idName);
            }
        }

        // Copy list of weighting
        List<float> currentRarityWeighting = ItemData.rarityWeighting;
        // Then we will modify it, because some pools are empty
        for (int test = allItemsByPool.Count - 1; test >= 0; test--)
        {
            // If a pool is empty ( no more common item for exemple )
            if (allItemsByPool[test].Count == 0)
            {
                float chanceToDivide = currentRarityWeighting[test];
                // We remove the empty pool in rarity weighting
                currentRarityWeighting.RemoveAt(test);
                chanceToDivide /= currentRarityWeighting.Count;

                for (int iWeighting = 0; iWeighting < currentRarityWeighting.Count; iWeighting++)
                {
                    currentRarityWeighting[iWeighting] += chanceToDivide;
                }
                // And we remove the pool
                allItemsByPool.RemoveAt(test);
                ItemPool.RemoveAt(test);
            }

        }

        float chanceForRarity = Seed.Range();
        float currentChance = 0;
        int indexRarity;
        for (indexRarity = 0; indexRarity < currentRarityWeighting.Count; indexRarity++)
        {
            currentChance += currentRarityWeighting[indexRarity];
            if (chanceForRarity < currentChance)
            {
                break;
            }
        }

        int indexInPool = Seed.Range(0, allItemsByPool[indexRarity].Count);
        item.idItemName = allItemsByPool[indexRarity][indexInPool];
        for (int poolIndex = 0; poolIndex < ItemPool.Count; poolIndex++)
        {
            var test = ItemPool[poolIndex].FirstOrDefault(x => x.idName == item.idItemName);
            if (test != null)
            {
                ItemPool[poolIndex].Remove(test);
                return;
            }
        }

    }
    public void RandomizeItem()
    {
        List<string> allItems = new();
        foreach (var itemInDb in database.datas)
        {
            allItems.Add(itemInDb.idName);
        }
        int indexRandom = UnityEngine.Random.Range(0, allItems.Count - 1);
        idItemName = allItems[indexRandom];
    }

    public void CreateItem()
    {
        itemEffect = LoadClass();

        ItemData data = database.GetItem(idItemName);
        Material matToRender = data.mat;
        Mesh meshToRender = data.mesh;
        price = (int)(data.RarityTier + 1) * PRICE_PER_RARITY;

        rarityColor = database.GetItemRarityColor(idItemName);

        this.GetComponentInChildren<MeshRenderer>().material = matToRender != null ? matToRender : this.GetComponentInChildren<MeshRenderer>().material;
        this.GetComponentInChildren<MeshFilter>().mesh = meshToRender != null ? meshToRender : this.GetComponentInChildren<MeshFilter>().mesh;

        itemDescription = GetComponent<ItemDescription>();
        itemDescription.SetDescription(idItemName);
        auraVFX.SetFloat("Orbs amount", (float)(data.RarityTier + 1));
        auraVFX.SetVector4("Color", rarityColor);
        auraVFX.Play();
    }
}