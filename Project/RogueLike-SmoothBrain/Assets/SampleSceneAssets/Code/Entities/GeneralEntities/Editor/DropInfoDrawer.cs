using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DropInfo))]
public class DropInfoDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        base.OnGUI(position, property, label);
    }
}
