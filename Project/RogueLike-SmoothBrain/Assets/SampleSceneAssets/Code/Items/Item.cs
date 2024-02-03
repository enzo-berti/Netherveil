using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System;
using System.Net;
using UnityEditor.PackageManager.UI;
using JetBrains.Annotations;

[Serializable]
public class Item : MonoBehaviour, IInterractable
{
    public string item;
    public float test;
    private void Start()
    {
        Debug.Log(item);
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
        Debug.Log($"Vous avez bien récupéré {LoadClass().GetType()})");
        Destroy(this.gameObject);
    }


    ItemEffect LoadClass()
    {
        return Assembly.GetExecutingAssembly().CreateInstance(item) as ItemEffect;
    }
}


[CustomEditor(typeof(Item))]
public class ItemEditor : Editor
{
    public static string ChosenName;
    SerializedProperty itemName;
    private void OnEnable()
    {
        itemName = serializedObject.FindProperty("item");
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

    public void OnDestroy()
    {
        Debug.Log("destroy");
    }
}