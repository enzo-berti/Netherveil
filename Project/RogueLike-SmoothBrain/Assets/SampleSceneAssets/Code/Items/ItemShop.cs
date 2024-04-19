using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

// This class is the item that is rendered in the 3D world
[Serializable]
public class ItemShop : MonoBehaviour, IInterractable
{
    [SerializeField] bool isRandomized = true;
    [SerializeField] ItemDatabase database;

    public Color RarityColor { get; private set; }
    public string idItemName;
    public string descriptionToDisplay;

    public static event Action<ItemEffect> OnRetrieved;

    Hero hero;
    Outline outline;

    // Item to add in the inventory
    ItemEffect itemToGive;
    PlayerInteractions playerInteractions;

    // Description displayed in the info box
    ItemDescription itemDescription;

    private void Awake()
    {
        if (isRandomized)
        {
            RandomizeItem();
        }

        itemToGive = LoadClass();
        Material matToRender = database.GetItem(idItemName).mat;
        Mesh meshToRender = database.GetItem(idItemName).mesh;
        RarityColor = database.GetItemRarityColor(idItemName);

        this.GetComponentInChildren<MeshRenderer>().material = matToRender != null ? matToRender : this.GetComponentInChildren<MeshRenderer>().material;
        this.GetComponentInChildren<MeshFilter>().mesh = meshToRender != null ? meshToRender : this.GetComponentInChildren<MeshFilter>().mesh;

        InitDescription();

        playerInteractions = GameObject.FindWithTag("Player").GetComponent<PlayerInteractions>();
        hero = playerInteractions.gameObject.GetComponent<Hero>();
        outline = GetComponent<Outline>();
        itemDescription = GetComponent<ItemDescription>();
    }

    private void Update()
    {
        Interraction();
    }

    public void Select()
    {
        outline.EnableOutline();
        itemDescription.TogglePanel(true);
    }

    public void Deselect()
    {
        outline.DisableOutline();
        itemDescription.TogglePanel(false);
    }

    private void Interraction()
    {
        bool isInRange = Vector2.Distance(playerInteractions.transform.position.ToCameraOrientedVec2(), transform.position.ToCameraOrientedVec2())
            <= hero.Stats.GetValue(Stat.CATCH_RADIUS);

        if (isInRange && !playerInteractions.InteractablesInRange.Contains(this))
        {
            playerInteractions.InteractablesInRange.Add(this);
        }
        else if (!isInRange && playerInteractions.InteractablesInRange.Contains(this))
        {
            playerInteractions.InteractablesInRange.Remove(this);
            Deselect();
        }
    }

    public void Interract()
    {
        itemToGive.Name = idItemName;
        GameObject.FindWithTag("Player").GetComponent<Hero>().Inventory.AddItem(itemToGive);
        Debug.Log($"Vous avez bien récupéré {itemToGive.Name}");
        Destroy(this.gameObject);
        playerInteractions.InteractablesInRange.Remove(this);
        DeviceManager.Instance.ApplyVibrations(0.1f, 0f, 0.1f);
        OnRetrieved?.Invoke(itemToGive);
    }

    ItemEffect LoadClass()
    {
        return Assembly.GetExecutingAssembly().CreateInstance(idItemName.GetPascalCase()) as ItemEffect;
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

    private void InitDescription()
    {
        descriptionToDisplay = database.GetItem(idItemName).Description;
        string[] splitDescription = descriptionToDisplay.Split(" ");
        string finalDescription = string.Empty;
        FieldInfo[] fieldOfItem = itemToGive.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

        for (int i = 0; i < splitDescription.Length; i++)
        {
            if (splitDescription[i].Length > 0 && splitDescription[i][0] == '{')
            {
                string[] splitCurrent = splitDescription[i].Split('{', '}');
                string valueToFind = splitCurrent[1];
                FieldInfo valueInfo = fieldOfItem.FirstOrDefault(x => x.Name == valueToFind);
                if (valueInfo != null)
                {
                    var memberValue = valueInfo.GetValue(itemToGive);
                    splitDescription[i] = memberValue.ToString();
                }
                else
                {
                    splitDescription[i] = "N/A";
                    Debug.LogWarning($"value : {valueToFind}, has not be found");
                }

            }
            finalDescription += splitDescription[i] + " ";
        }
        descriptionToDisplay = finalDescription;
    }
}