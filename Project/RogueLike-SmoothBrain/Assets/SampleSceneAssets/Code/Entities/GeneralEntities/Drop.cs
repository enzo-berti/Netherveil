using System;
using System.Collections.Generic;
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
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Create property container element.
        var container = new VisualElement();

        // Create property fields.
        var nameField = new PropertyField(property.FindPropertyRelative("dropList"), "Drops");

        // Add fields to the container.
        container.Add(nameField);

        return container;
    }
}
#endif