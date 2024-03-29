using System;
using System.Collections.Generic;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class Drop
{
    [SerializeField] List<DropInfo> dropList = new();
    public void DropLoot(Vector3 position)
    {
        foreach (DropInfo dropInfo in dropList)
        {
            if (dropInfo.isChanceShared)
            {
                if (UnityEngine.Random.value <= dropInfo.chance)
                {
                    for (int i = 0; i < dropInfo.Quantity; i++)
                    {
                        GameObject.Instantiate(dropInfo.loot, position, Quaternion.identity);
                    }
                }
            }
            else
            {
                for (int i = 0; i < dropInfo.Quantity; i++)
                {
                    if (UnityEngine.Random.value <= dropInfo.chance)
                    {
                        GameObject.Instantiate(dropInfo.loot, position, Quaternion.identity);
                    }
                }
            }

        }
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(Drop))]
public class DropDrawerUIE : PropertyDrawer
{
    SerializedProperty dropProperty;
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        dropProperty = property.FindPropertyRelative("dropList");
        EditorGUILayout.PropertyField(dropProperty, label);
    }
}
#endif