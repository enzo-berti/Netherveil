using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

// This class is the item that is rendered in the 3D world
[Serializable]
public class Item : MonoBehaviour, IInterractable
{
    [SerializeField] bool isRandomized = true;
    [SerializeField] ItemDatabase database;

    public Color RarityColor { get; private set; }
    public string idItemName;
    public string descriptionToDisplay;

    public static event Action<ItemEffect> OnRetrieved;

    Hero hero;
    Outline outline;
    GameObject meshObject;
    ItemEffect itemToGive;
    PlayerInteractions playerInteractions;
    ItemDescription itemDescription;

    private void Awake()
    {
        if (isRandomized)
        {
            RandomizeItem(this);
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
        meshObject = this.GetComponentInChildren<MeshRenderer>().gameObject;
        outline = GetComponent<Outline>();
        itemDescription = GetComponent<ItemDescription>();
    }

    private void Update()
    {
        Interraction();
        FloatingAnimation();
    }

    private void FloatingAnimation()
    {
        Vector3 updatePos = meshObject.transform.position;
        updatePos.y += Mathf.Sin(Time.time) * 0.0009f;
        meshObject.transform.position = updatePos;
        meshObject.transform.Rotate(new Vector3(0, 0.5f, 0));
    }

    private void Interraction()
    {
        bool isInRange = Vector2.Distance(playerInteractions.transform.position.ToCameraOrientedVec2(), transform.position.ToCameraOrientedVec2()) 
            <= hero.Stats.GetValue(Stat.CATCH_RADIUS);

        if (isInRange && !playerInteractions.ItemsInRange.Contains(this))
        {
            playerInteractions.ItemsInRange.Add(this);
        }
        else if (!isInRange && playerInteractions.ItemsInRange.Contains(this))
        {
            playerInteractions.ItemsInRange.Remove(this);
            outline.DisableOutline();
            itemDescription.TogglePanel(false);
        }
    }

    public void Interract()
    {
        itemToGive.Name = idItemName;
        GameObject.FindWithTag("Player").GetComponent<Hero>().Inventory.AddItem(itemToGive);
        Debug.Log($"Vous avez bien récupéré {itemToGive.GetType()}");
        Destroy(this.gameObject);
        playerInteractions.ItemsInRange.Remove(this);
        DeviceManager.Instance.ApplyVibrations(0.1f, 0f, 0.1f);
        OnRetrieved?.Invoke(itemToGive);
    }

    ItemEffect LoadClass()
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
        int indexRandom = Seed.Range(0, allItems.Count - 1);
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

    private void InitDescription()
    {
        descriptionToDisplay = database.GetItem(idItemName).Description;
        string[] splitDescription = descriptionToDisplay.Split(" ");
        string finalDescription = string.Empty;
        FieldInfo[] fieldOfItem = itemToGive.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
        //Debug.Log(itemToGive.GetType().Name);
        //foreach (var test in fieldOfItem)
        //{
        //    Debug.Log(test.Name);
        //}
        for (int i = 0; i < splitDescription.Length; i++)
        {
            if (splitDescription[i].Length > 0 && splitDescription[i][0] == '{')
            {
                string[] splitCurrent = splitDescription[i].Split('{', '}', '.');
                string valueToFind = splitCurrent[1];
                FieldInfo valueInfo = fieldOfItem.FirstOrDefault(x => x.Name == valueToFind);
                if (valueInfo != null)
                {
                    var memberValue = valueInfo.GetValue(itemToGive);
                    if (splitCurrent.Length > 2 && splitCurrent[2] == "%")
                    {
                        float memberFloat = (float)memberValue;
                        memberFloat *= 100;
                        memberValue = memberFloat;
                    }
                    splitDescription[i] = memberValue.ToString();
                    // CHANGE THAT HOLY
                    if (splitCurrent.Length > 2 && splitCurrent[2] == "%")
                    {
                        splitDescription[i] += "%";
                    }
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

#if UNITY_EDITOR
[CustomEditor(typeof(Item))]
public class ItemEditor : Editor
{
    public static string ChosenName;
    SerializedProperty itemName;
    SerializedProperty databaseProperty;
    SerializedProperty isRandomizedProperty;
    private void OnEnable()
    {
        itemName = serializedObject.FindProperty("idItemName");
        databaseProperty = serializedObject.FindProperty("database");
        isRandomizedProperty = serializedObject.FindProperty("isRandomized");
        ChosenName = itemName.stringValue;
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawScript();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(isRandomizedProperty);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("idName : ", EditorStyles.boldLabel, GUILayout.Width(80));
        if (GUILayout.Button(ChosenName))
        {
            EditorWindow.GetWindow<ResearchItemWindow>("Select Item");
        }

        if (GUILayout.Button("Randomize item"))
        {
            Item.RandomizeItem((Item)target);
            ChosenName = (target as Item).idItemName;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(databaseProperty, new GUIContent("Database : "));
        EditorGUILayout.EndHorizontal();

        itemName.stringValue = ChosenName;
        serializedObject.ApplyModifiedProperties();

    }

    void DrawScript()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginDisabledGroup(true);
        MonoScript script = MonoScript.FromMonoBehaviour((Item)target);
        EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();
    }
}

public class ResearchItemWindow : EditorWindow
{
    ItemDatabase database;
    string search;


    private void OnEnable()
    {
        database = Resources.Load<ItemDatabase>("ItemDatabase");
        search = string.Empty;
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        search = EditorGUILayout.TextField(search);
        EditorGUILayout.EndHorizontal();
        foreach (var item in database.datas.Select(x => x.idName).Where(x => x.ToLower().Contains(search)))
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(item))
            {
                ItemEditor.ChosenName = item;
                Close();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif
