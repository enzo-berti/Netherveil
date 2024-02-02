using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CreateItemWindow : EditorWindow
{
    ItemDatabase database;
    ItemData item = new ItemData();
    public static void OpenWindow()
    {
        GetWindow<CreateItemWindow>("CreateItem");
    }

    private void OnEnable()
    {
        database = Resources.Load<ItemDatabase>("ItemDatabase");
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Id name : ");
        item.idName = EditorGUILayout.TextField(item.idName);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Description : ", GUILayout.Height(30));
        item.Description = EditorGUILayout.TextField(item.Description, GUILayout.Height(30));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Type : ");
        item.Type = (ItemData.ItemType)EditorGUILayout.EnumPopup(item.Type);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Rarity : ");
        item.RarityTier = (ItemData.Rarity)EditorGUILayout.EnumPopup(item.RarityTier);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUI.color = Color.green;
        if (GUILayout.Button("Save"))
        {
            database.datas.Add(item);
            CreateScript();
            Close();
        }
        GUI.color = Color.white;
        EditorGUILayout.EndHorizontal();

    }

    void CreateScript()
    {
        string itemName = item.idName.GetCamelCase();
        string path = Application.dataPath + "/SampleSceneAssets/Code/Items/" + (item.Type == ItemData.ItemType.PASSIVE ? "PassiveItems" : "ActiveItems") + $"/{itemName}.cs";
        Debug.Log(path);
        StreamReader sr = new StreamReader(path + "/../../ItemSample.txt");
        StreamWriter sw = new StreamWriter(path);
        string line;
        while ((line = sr.ReadLine()) != null)
        {
            List<string> splitLine = line.Split(' ').ToList();
            string wordToAdd = string.Empty;
            string finalLine = string.Empty;
            foreach (var word in splitLine)
            {
                wordToAdd = word;
                switch (word)
                {
                    case "classSampleName":
                        wordToAdd = itemName;
                        break;
                    case "sampleInterface":
                        wordToAdd = (item.Type == ItemData.ItemType.PASSIVE ? "IPassiveItem" : "IActiveItem");
                        break;
                }
                finalLine += wordToAdd + ' ';
            }
            sw.WriteLine(finalLine);
        }
        sr.Close();
        sw.Close();

        AssetDatabase.Refresh();
    }
}
