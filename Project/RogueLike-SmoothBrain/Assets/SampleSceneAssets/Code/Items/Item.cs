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
    public string idItemName;
    public string descriptionToDisplay;
    ItemDatabase database;
    [SerializeField] Mesh defaultMesh;
    [SerializeField] Material defaultMat;
    [SerializeField] Material outlineMaterial;
    ItemEffect itemToGive;
    public static event Action<ItemEffect> onRetrieved;
    PlayerInteractions playerInteractions;
    Hero hero;

    bool isInItemZone = false;
    private void Awake()
    {
        database = Resources.Load<ItemDatabase>("ItemDatabase");
        RandomizeItem(this);
        itemToGive = LoadClass();
        Material matToRender = database.GetItem(idItemName).mat;
        Mesh meshToRender = database.GetItem(idItemName).mesh;
        this.GetComponent<MeshRenderer>().material = matToRender != null ? matToRender : defaultMat;
        this.GetComponent<MeshFilter>().mesh = meshToRender != null ? meshToRender : defaultMesh;
        InitDescription();
        playerInteractions = GameObject.FindWithTag("Player").GetComponent<PlayerInteractions>();
        hero = playerInteractions.gameObject.GetComponent<Hero>();
    }

    private void Update()
    {
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;
        Vector3 tmp = (cameraForward * playerInteractions.transform.position.z + cameraRight * playerInteractions.transform.position.x);
        Vector2 playerPos = new Vector2(tmp.x, tmp.z);
        tmp = (cameraForward * this.transform.position.z + cameraRight * this.transform.position.x);
        Vector2 itemPos = new Vector2(tmp.x, tmp.z);

        bool isInRange = Vector2.Distance(playerPos, itemPos) <= hero.Stats.GetValue(Stat.CATCH_RADIUS);

        if (isInRange && !playerInteractions.interactablesInRange.Contains(this))
        {
            playerInteractions.interactablesInRange.Add(this);
        }
        else if (!isInRange && playerInteractions.interactablesInRange.Contains(this))
        {
            playerInteractions.interactablesInRange.Remove(this);

            var meshRenderer = gameObject.GetComponent<MeshRenderer>();
            GetComponent<ItemDescription>().TogglePanel(false);
            if (meshRenderer.materials.Length > 1)
            {
                List<Material> finalMaterial = new()
                    {
                        meshRenderer.material
                    };
                meshRenderer.SetMaterials(finalMaterial);
            }
        }
    }
    public void Interract()
    {
        itemToGive.Name = idItemName;
        GameObject.FindWithTag("Player").GetComponent<Hero>().Inventory.AddItem(itemToGive);
        Debug.Log($"Vous avez bien récupéré {itemToGive.GetType()}");
        Destroy(this.gameObject);
        playerInteractions.interactablesInRange.Remove(this);
        onRetrieved?.Invoke(itemToGive);
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
        FieldInfo[] fieldOfItem = itemToGive.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        for (int i = 0; i < splitDescription.Length; i++)
        {
            if (splitDescription[i][0] == '{')
            {
                string valueToFind = splitDescription[i].Split('{', '}')[1];
                FieldInfo valueInfo = fieldOfItem.FirstOrDefault(x => x.Name == valueToFind);
                if (valueInfo != null)
                {
                    splitDescription[i] = valueInfo.GetValue(itemToGive).ToString();
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
    SerializedProperty defaultMeshProperty;
    SerializedProperty defaultMatProperty;
    SerializedProperty outlineMatProperty;
    private void OnEnable()
    {
        itemName = serializedObject.FindProperty("idItemName");
        defaultMeshProperty = serializedObject.FindProperty("defaultMesh");
        defaultMatProperty = serializedObject.FindProperty("defaultMat");
        outlineMatProperty = serializedObject.FindProperty("outlineMaterial");
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

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(defaultMeshProperty, new GUIContent("Default Mesh : "));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(defaultMatProperty, new GUIContent("Default Material : "));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(outlineMatProperty, new GUIContent("Outline Material : "));
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
