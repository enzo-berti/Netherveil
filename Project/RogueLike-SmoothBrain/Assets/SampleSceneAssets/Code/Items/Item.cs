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

    [SerializeField] private bool isRandomized = true;
    [SerializeField] private ItemDatabase database;

    private ItemEffect itemData;
    private string idItem = string.Empty;
    private Color rarityColor = Color.white;

    private ItemDescription itemDescription;

    public Color RarityColor => rarityColor;
    public ItemEffect ItemData => itemData;
    public ItemDatabase Database => database;
    public string IdItem => idItem;

    private void Start()
    {
        if (isRandomized)
        {
            RandomizeItem(this);
        }

        itemData = LoadClass();
        Material matToRender = database.GetItem(idItem).mat;
        Mesh meshToRender = database.GetItem(idItem).mesh;
        rarityColor = database.GetItemRarityColor(idItem);

        this.GetComponentInChildren<MeshRenderer>().material = matToRender != null ? matToRender : this.GetComponentInChildren<MeshRenderer>().material;
        this.GetComponentInChildren<MeshFilter>().mesh = meshToRender != null ? meshToRender : this.GetComponentInChildren<MeshFilter>().mesh;

        itemDescription = GetComponent<ItemDescription>();
        itemDescription.SetDescription(idItem);
    }

    private ItemEffect LoadClass()
    {
        return Assembly.GetExecutingAssembly().CreateInstance(idItem.GetPascalCase()) as ItemEffect;
    }

    static public void RandomizeItem(Item item)
    {
        List<string> allItems = new();
        foreach (var itemInDb in item.database.datas)
        {
            allItems.Add(itemInDb.idName);
        }
        int indexRandom = Seed.Range(0, allItems.Count);
        item.idItem = allItems[indexRandom];
    }

    public void RandomizeItem()
    {
        List<string> allItems = new();
        foreach (var itemInDb in database.datas)
        {
            allItems.Add(itemInDb.idName);
        }
        int indexRandom = UnityEngine.Random.Range(0, allItems.Count - 1);
        idItem = allItems[indexRandom];
    }
}