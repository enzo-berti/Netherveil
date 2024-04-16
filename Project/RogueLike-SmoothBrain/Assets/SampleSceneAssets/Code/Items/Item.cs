using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Generation;

// This class is the item that is rendered in the 3D world
[Serializable]
public class Item : MonoBehaviour
{
    [SerializeField] private bool isRandomized = true;
    [SerializeField] private ItemDatabase database;

    public static event Action<ItemEffect> OnRetrieved;

    private ItemEffect itemData;
    public string idItemName;

    private ItemDescription itemDescription;

    public Color RarityColor { get; private set; }
    public ItemEffect ItemData => itemData;
    public ItemDatabase Database => database;

    private void Start()
    {
        if (isRandomized)
        {
            RandomizeItem(this);
        }

        itemData = LoadClass();
        Material matToRender = database.GetItem(idItemName).mat;
        Mesh meshToRender = database.GetItem(idItemName).mesh;
        RarityColor = database.GetItemRarityColor(idItemName);

        this.GetComponentInChildren<MeshRenderer>().material = matToRender != null ? matToRender : this.GetComponentInChildren<MeshRenderer>().material;
        this.GetComponentInChildren<MeshFilter>().mesh = meshToRender != null ? meshToRender : this.GetComponentInChildren<MeshFilter>().mesh;

        itemDescription = GetComponent<ItemDescription>();
        itemDescription.SetDescription(idItemName);
    }

    private ItemEffect LoadClass()
    {
        return Assembly.GetExecutingAssembly().CreateInstance(idItemName.GetPascalCase()) as ItemEffect;
    }

    static public void RandomizeItem(Item item)
    {
        List<string> allItems = new();
        foreach (var itemInDb in item.database.datas)
        {
            allItems.Add(itemInDb.idName);
        }
        int indexRandom = Seed.Range(0, allItems.Count);
        item.idItemName = allItems[indexRandom];
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