using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DialogueSystemUI))]
public class DialogueSystemUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }
}
