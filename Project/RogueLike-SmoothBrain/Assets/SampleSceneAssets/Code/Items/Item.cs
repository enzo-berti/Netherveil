using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Generation;

// This class is the item that is rendered in the 3D world
[Serializable]
public class Item : MonoBehaviour
{
    public static event Action<ItemEffect> OnRetrieved;
    public static float priceCoef = 1.0f;

    [SerializeField] private bool isRandomized = true;
    [SerializeField] private ItemDatabase database;

    private ItemEffect itemEffect;
    private Color rarityColor = Color.white;
    public string idItemName = string.Empty;
    private int price;

    private ItemDescription itemDescription;
    public Color RarityColor => rarityColor;
    public ItemEffect ItemData => itemEffect;
    public ItemDatabase Database => database;
    public static List<ItemData> ItemPool;
    public int Price => price;

    private void Start() 
    {
        if(ItemPool == null)
        {
            ItemPool = new List<ItemData>();
            foreach(var itemData in database.datas)
            {
                ItemPool.Add(itemData);
            }
        }
        if (isRandomized)
        {
            RandomizeItem(this);
        }

        itemEffect = LoadClass();

        ItemData data = database.GetItem(idItemName);
        Material matToRender = data.mat;
        Mesh meshToRender = data.mesh;
        price = data.price;

        rarityColor = database.GetItemRarityColor(idItemName);

        this.GetComponentInChildren<MeshRenderer>().material = matToRender != null ? matToRender : this.GetComponentInChildren<MeshRenderer>().material;
        this.GetComponentInChildren<MeshFilter>().mesh = meshToRender != null ? meshToRender : this.GetComponentInChildren<MeshFilter>().mesh;

        itemDescription = GetComponent<ItemDescription>();
        itemDescription.SetDescription(idItemName);
    }

    public static void InvokeOnRetrieved(ItemEffect effect)
    {
        OnRetrieved?.Invoke(effect);
    }

    private ItemEffect LoadClass()
    {
        return Assembly.GetExecutingAssembly().CreateInstance(idItemName.GetPascalCase()) as ItemEffect;
    }

    static public void RandomizeItem(Item item)
    {
        if(ItemPool.Count > 0)
        {
            List<string> allItems = new();
            foreach (var itemInPool in ItemPool)
            {
                allItems.Add(itemInPool.idName);
            }
            int indexRandom = Seed.Range(0, allItems.Count);
            item.idItemName = allItems[indexRandom];
            ItemPool.RemoveAt(indexRandom);

        }
        else
        {
            item.idItemName = "MonsterHeart";
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
}