using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class WindowItemDatabase : EditorWindow
{
    ItemDatabase database;
    List<ItemData> searchItems = new List<ItemData>();
    Vector2 scrollPos = Vector2.zero;
    string search = "";

    [MenuItem("Tools/ItemDatabase")]
    public static void OpenWindow()
    {
        GetWindow<WindowItemDatabase>("Item Database");
    }

    private void OnEnable()
    {
        database = Resources.Load<ItemDatabase>("ItemDatabase");
    }

    private void OnGUI()
    {
        SearchInDatabase();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        // Search Field
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Search :", GUILayout.Width(60));
        search = GUILayout.TextField(search);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUI.color = Color.green;
        if (GUILayout.Button("Add Item"))
        {
            GetWindow<CreateItemWindow>("Create Item");
        }
        GUI.color = Color.white;
        EditorGUILayout.EndHorizontal();
        // Infos
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Id_Name");
        EditorGUILayout.LabelField("Rarity");
        EditorGUILayout.LabelField("Type");
        EditorGUILayout.LabelField("Description");
        EditorGUILayout.EndHorizontal();

        
        
        for (int i = 0; i < searchItems.Count; i++)
        {
            ItemData item = searchItems[i];

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.LabelField(item.idName.SeparateAllCase());
            item.RarityTier = (ItemData.Rarity)EditorGUILayout.EnumPopup(item.RarityTier);
            item.Type = (ItemData.ItemType)EditorGUILayout.EnumPopup(item.Type);
            item.Description = EditorGUILayout.TextField(item.Description, GUILayout.Height(100));
            GUI.color = Color.red;
            if (GUILayout.Button("X", GUILayout.Width(50)))
            {
                DeleteInDatabase(item);
            }
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }
    void SearchInDatabase()
    {
        searchItems = database.datas.Where(item => item.idName.ToLower().Contains(search.ToLower()) || item.idName.ToLower().Contains(search.ToLower())).ToList();
    }

    void DeleteInDatabase(ItemData item)
    {
        database.datas.Remove(item);
        string itemName = item.idName.GetCamelCase();
        string path = Application.dataPath + "/SampleSceneAssets/Code/Items/" + (item.Type == ItemData.ItemType.PASSIVE ? "PassiveItems" : "ActiveItems") + $"/{itemName}.cs";
        File.Delete(path);
        AssetDatabase.Refresh();
    }
}
