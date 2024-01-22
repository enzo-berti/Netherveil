using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DialogueTrigger))]
public class DialogueTriggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (Application.isPlaying)
        {
            DialogueTrigger dialogueTrigger = target as DialogueTrigger;

            EditorGUILayout.Space();
            if (GUILayout.Button("Start dialogue"))
            {
                dialogueTrigger.TriggerDialogue();
            }
        }
    }
}
