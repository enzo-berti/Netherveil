using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class Item : MonoBehaviour, IInterractable
{
    public string idItemName;
    ItemDatabase database;
    private void Awake()
    {
        database = Resources.Load<ItemDatabase>("ItemDatabase");
        this.GetComponent<MeshRenderer>().material = database.GetItem(idItemName).mat;
        this.GetComponent<MeshFilter>().mesh = database.GetItem(idItemName).mesh;

        RandomizeItem(this);
        Debug.Log(idItemName);
    }
    private void Update()
    {
        if (Vector2.Distance(GameObject.FindWithTag("Player").transform.position, transform.position) < 10)
        {
            Interract();
        }
    }
    public void Interract()
    {
        GameObject.FindWithTag("Player").GetComponent<Hero>().Inventory.AddItem(LoadClass());
        Debug.Log($"Vous avez bien récupéré {LoadClass().GetType()}");
        Destroy(this.gameObject);
    }

    ItemEffect LoadClass()
    {
        return Assembly.GetExecutingAssembly().CreateInstance(idItemName) as ItemEffect;
    }

    static public void RandomizeItem(Item item)
    {
        List<string> allItems = new();
        ItemDatabase db = Resources.Load<ItemDatabase>("ItemDatabase");
        foreach (var itemInDb in db.datas)
        {
            allItems.Add(itemInDb.idName);
        }
        int indexRandom = UnityEngine.Random.Range(0, allItems.Count - 1);
        item.idItemName = allItems[indexRandom];
        Debug.Log("Random askip");
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
        Debug.Log("Random askip");

    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Item))]
public class ItemEditor : Editor
{
    public static string ChosenName;
    SerializedProperty itemName;
    private void OnEnable()
    {
        itemName = serializedObject.FindProperty("idItemName");
        ChosenName = itemName.stringValue;
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawScript();
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
