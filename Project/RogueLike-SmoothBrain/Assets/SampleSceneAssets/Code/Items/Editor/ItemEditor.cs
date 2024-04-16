using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Item))]
public class ItemEditor : Editor
{
    public static string ChosenName;
    SerializedProperty itemName;
    SerializedProperty databaseProperty;
    SerializedProperty isRandomizedProperty;
    private void OnEnable()
    {
        itemName = serializedObject.FindProperty("idItem");
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
            ChosenName = (target as Item).IdItem;
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