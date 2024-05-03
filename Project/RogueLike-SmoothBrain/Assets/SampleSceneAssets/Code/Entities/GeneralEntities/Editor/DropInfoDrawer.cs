using Unity.VisualScripting;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomPropertyDrawer(typeof(DropInfo))]
public class DropInfoDrawer : PropertyDrawer
{
    enum SerializeType
    {
        NONE,
        INT,
        RANGE,
        FLOAT
    }
    SerializedProperty lootProperty;
    SerializedProperty chanceProperty;
    SerializedProperty quantityProperty;

    SerializedProperty isChanceSharedProperty;
    SerializedProperty decreasingValueProperty;

    int nbMember = 0;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        nbMember = 0;
        EditorGUI.BeginProperty(position, label, property);
        lootProperty = property.FindPropertyRelative("loot");
        chanceProperty = property.FindPropertyRelative("chance");
        quantityProperty = property.FindPropertyRelative("quantity");
        isChanceSharedProperty = property.FindPropertyRelative("isChanceShared");
        decreasingValueProperty = property.FindPropertyRelative("decreasingValuePerDrop");
        label.text = lootProperty.objectReferenceValue.name;
        Rect foldoutBox = new Rect(position.min.x, position.min.y, position.size.x, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(foldoutBox, property.isExpanded, label);
        if(property.isExpanded)
        {
            DrawMember(position, lootProperty);
            DrawMember(position, chanceProperty, SerializeType.RANGE);
            DrawMember(position, quantityProperty);
            DrawMember(position, isChanceSharedProperty);
            if(!isChanceSharedProperty.boolValue)
            {
                DrawMember(position, decreasingValueProperty, SerializeType.RANGE);
            }
        }

        EditorGUI.EndProperty();
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int totalLine = 1;
        if (property.isExpanded)
        {
            totalLine += 4;
            if (!property.FindPropertyRelative("isChanceShared").boolValue) totalLine += 1;
        }
        return (EditorGUIUtility.singleLineHeight + 2) * totalLine;
    }

    private void DrawMember(Rect position, SerializedProperty propertyToDraw, SerializeType type = SerializeType.NONE)
    {
        nbMember++;
        EditorGUI.indentLevel++;
        float posX = position.min.x;
        float posY = position.min.y + (2 + EditorGUIUtility.singleLineHeight) * nbMember;
        float width = position.size.x;
        float height = EditorGUIUtility.singleLineHeight;

        Rect drawArea = new Rect(posX, posY, width, height);

        switch(type)
        {
            case SerializeType.INT:
                propertyToDraw.intValue = EditorGUI.IntField(drawArea, new GUIContent(propertyToDraw.name), propertyToDraw.intValue);
                break;
            case SerializeType.RANGE:
                propertyToDraw.floatValue = EditorGUI.Slider(drawArea, new GUIContent(propertyToDraw.name), propertyToDraw.floatValue, 0, 1);
                break;
            default:
                EditorGUI.PropertyField(drawArea, propertyToDraw);
                break;
        }
        
        EditorGUI.indentLevel--;
    }
}
