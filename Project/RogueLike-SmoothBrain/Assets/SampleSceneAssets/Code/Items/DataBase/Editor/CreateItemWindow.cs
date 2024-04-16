using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class CreateItemWindow : EditorWindow
{
    ItemDatabase database;
    ItemData item = new ItemData();
    float activeCooldown = 0;
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

        if(item.Type == ItemData.ItemType.ACTIVE || item.Type == ItemData.ItemType.PASSIVE_ACTIVE)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Cooldown : ");
            activeCooldown = EditorGUILayout.FloatField(activeCooldown);
            EditorGUILayout.EndHorizontal();
        }
        
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
            EditorUtility.SetDirty(database);
            AssetDatabase.SaveAssetIfDirty(database);
        }
        GUI.color = Color.white;
        EditorGUILayout.EndHorizontal();

    }

    void CreateScript()
    {
        string itemName = item.idName.GetPascalCase();
        string itemType = string.Empty;
        switch (item.Type)
        {
            case ItemData.ItemType.PASSIVE:
                itemType = "PassiveItems";
                break;
            case ItemData.ItemType.ACTIVE:
                itemType = "ActiveItems";
                break;
            case ItemData.ItemType.PASSIVE_ACTIVE:
                itemType = "ActivePassiveItems";
                break;
            default:
                break;
        }
        string path = Application.dataPath + "/SampleSceneAssets/Code/Items/" + itemType + $"/{itemName}.cs";
        StreamReader sr = new StreamReader(path + "/../../ItemSample.txt");
        StreamWriter sw = new StreamWriter(path);
        List<Type> typeList = new List<Type>();
        string line;
        while ((line = sr.ReadLine()) != null)
        {
            List<string> splitLine = line.Split(' ').ToList();
            
            string finalLine = string.Empty;
            foreach (var word in splitLine)
            {
                string wordToAdd = word;
                switch (word)
                {
                    case "classSampleName":
                        wordToAdd = itemName;
                        break;
                    case "sampleInterface":
                        switch(item.Type)
                        {
                            case ItemData.ItemType.PASSIVE:
                                wordToAdd = "IPassiveItem";
                                typeList.Add(typeof(IPassiveItem));
                                break;
                            case ItemData.ItemType.ACTIVE:
                                typeList.Add(typeof(IActiveItem));
                                wordToAdd = "IActiveItem";
                                break;
                            case ItemData.ItemType.PASSIVE_ACTIVE:
                                typeList.Add(typeof(IPassiveItem));
                                typeList.Add(typeof(IActiveItem));
                                wordToAdd = "IPassiveItem, IActiveItem";
                                break;
                        }
                        break;
                    case "functionSample":
                        wordToAdd = string.Empty;
                        for(int i = 0; i < typeList.Count; i++)
                        {
                            string methodToWrite = "    ";
                            for(int j = 0; j < typeList[i].GetMethods().Length; j++)
                            {
                                var method = typeList[i].GetMethods()[j];
                                if(!method.IsStatic)
                                {
                                    if (method.Name.Split("_")[0] == "get" || method.Name.Split("_")[0] == "set")
                                    {
                                        var name = method.Name.Split("_")[1];
                                        var returnType = method.ReturnType;
                                        methodToWrite += "public " + returnType + " " + name + " { ";
                                        do
                                        {
                                            methodToWrite += method.Name.Split("_")[0] + "; ";
                                            j++;
                                            method = typeList[i].GetMethods()[j];
                                        } while (method.Name.Split("_").Length > 1 && (method.Name.Split("_")[1] == name || method.Name.Split("_")[1] == name));
                                        j--;
                                        methodToWrite += "} = " + activeCooldown + ";\n\n    ";
                                    }
                                    else
                                    {
                                        if (method.IsPublic) methodToWrite += "public ";
                                        else if (method.IsPrivate) methodToWrite += "private ";
                                        else methodToWrite += "protected ";

                                        Type type = method.ReturnType;
                                        string typeString = type.ToString() == "System.Void" ? "void" : type.ToString();
                                        methodToWrite += typeString + " ";
                                        methodToWrite += method.Name + "(";
                                        if (method.IsAbstract)
                                        {
                                            methodToWrite += ")\n    {\n        throw new System.NotImplementedException();\n    }\n    ";
                                        }
                                    }
                                }
                                
                                
                            }
                            sw.WriteLine(methodToWrite);
                        }
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
