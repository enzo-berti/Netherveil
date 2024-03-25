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
    const int SizeArea = 100;

    [UnityEditor.MenuItem("Tools/ItemDatabase")]
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
        EditorGUILayout.LabelField("Id_Name", GUILayout.Width(SizeArea), GUILayout.ExpandWidth(true));
        EditorGUILayout.LabelField("Rarity", GUILayout.Width(SizeArea), GUILayout.ExpandWidth(true));
        EditorGUILayout.LabelField("Type", GUILayout.Width(SizeArea), GUILayout.ExpandWidth(true));
        EditorGUILayout.LabelField("Description", GUILayout.Width(SizeArea*2), GUILayout.ExpandWidth(true));
        EditorGUILayout.LabelField("Icon", GUILayout.Width(SizeArea/3), GUILayout.ExpandWidth(true));
        EditorGUILayout.LabelField("Material", GUILayout.Width(SizeArea), GUILayout.ExpandWidth(true));
        EditorGUILayout.LabelField("Mesh", GUILayout.Width(SizeArea), GUILayout.ExpandWidth(true));
        EditorGUILayout.EndHorizontal();

        
        
        for (int i = 0; i < searchItems.Count; i++)
        {
            ItemData item = searchItems[i];

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.LabelField(item.idName.SeparateAllCase(), GUILayout.Width(SizeArea), GUILayout.ExpandWidth(true));
            item.RarityTier = (ItemData.Rarity)EditorGUILayout.EnumPopup(item.RarityTier, GUILayout.Width(SizeArea), GUILayout.ExpandWidth(true));
            item.Type = (ItemData.ItemType)EditorGUILayout.EnumPopup(item.Type, GUILayout.Width(SizeArea), GUILayout.ExpandWidth(true));
            item.Description = EditorGUILayout.TextArea(item.Description, GUILayout.Height(100), GUILayout.Width(SizeArea*2), GUILayout.ExpandWidth(true));
            item.icon = (Texture)EditorGUILayout.ObjectField("", item.icon, typeof(Texture), false, GUILayout.Width(SizeArea/3), GUILayout.ExpandWidth(true));
            item.mat = (Material)EditorGUILayout.ObjectField("", item.mat, typeof(Material), false, GUILayout.Width(SizeArea), GUILayout.ExpandWidth(true));
            item.mesh = (Mesh)EditorGUILayout.ObjectField("", item.mesh, typeof(Mesh), false, GUILayout.Width(SizeArea), GUILayout.ExpandWidth(true));
            GUI.color = Color.red;
            if (GUILayout.Button("X", GUILayout.Width(50)))
            {
                DeleteInDatabase(item);
            }
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        EditorUtility.SetDirty(database);
    }
    void SearchInDatabase()
    {
        searchItems = database.datas.Where(item => item.idName.SeparateAllCase().ToLower().Contains(search.ToLower()) || item.idName.SeparateAllCase().ToLower().Contains(search.ToLower())).ToList();
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
