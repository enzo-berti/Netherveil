using UnityEditor;
using UnityEngine;

public class RoomTool : EditorWindow 
{
    string myString = "Hello World";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;
    GameObject room;

    [MenuItem("Tools/Room/Create")]
    public static void CreateRoom()
    {
        EditorWindow.GetWindow(typeof(RoomTool));
    }

    void OnGUI()
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        room = EditorGUILayout.ObjectField("Room prefab", room, typeof(GameObject), false) as GameObject;

        myString = EditorGUILayout.TextField("Text Field", myString);

        groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        myBool = EditorGUILayout.Toggle("Toggle", myBool);
        myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
        EditorGUILayout.EndToggleGroup();

        EditorGUILayout.EndVertical();
    }
}
