using System;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;

// Si vous ajoutez un truc, ajoutez le à la fin
public enum Stat
{
    HP,
    ATK,
    ATK_COEFF,
    ATK_RANGE,
    FIRE_RATE,
    SPEED,
    CATCH_RADIUS,
    VISION_RANGE,
    CRIT_RATE,
    CRIT_DAMAGE,
    MAX_HP,
    CORRUPTION,
    LIFE_STEAL,
    HEAL_COEFF,
    KNOCKBACK_COEFF,
    STAGGER_DURATION
}

[Serializable]
public class StatInfo
{
    public Stat stat;
    public float value;
    public bool hasMaxStat;
    public Stat maxStat;
}


#if UNITY_EDITOR
///// Affichage carré dans l'éditeur /////

[CustomPropertyDrawer(typeof(StatInfo))]
public class StatInfoDrawerUIE : PropertyDrawer
{
    SerializedProperty statProperty;
    SerializedProperty valueProperty;
    SerializedProperty hasMaxStatProperty;
    SerializedProperty maxStatProperty;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        statProperty = property.FindPropertyRelative("stat");
        valueProperty = property.FindPropertyRelative("value");
        hasMaxStatProperty = property.FindPropertyRelative("hasMaxStat");
        maxStatProperty = property.FindPropertyRelative("maxStat");
        Rect foldoutBox = new Rect(position.min.x, position.min.y, position.size.x, EditorGUIUtility.singleLineHeight);
        property.isExpanded =  EditorGUI.Foldout(foldoutBox, property.isExpanded, label);
        if(property.isExpanded)
        {
            DrawStat(position);
            DrawValue(position);
            DrawBoolMaxStat(position);
            if(hasMaxStatProperty.boolValue)
            {
                DrawMaxStat(position);
            }
        }
        EditorGUI.EndProperty();
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int totalLine = 1;
        if(property.isExpanded)
        {
            totalLine += 4;
        }
        return EditorGUIUtility.singleLineHeight * totalLine;
    }
    private void DrawStat(Rect position)
    {
        float posX = position.min.x;
        float posY = position.min.y + EditorGUIUtility.singleLineHeight;
        float width = position.size.x;
        float height = EditorGUIUtility.singleLineHeight;

        Rect drawArea = new Rect(posX, posY, width, height);
        EditorGUI.PropertyField(drawArea, statProperty);
    }

    private void DrawValue(Rect position)
    {
        float posX = position.min.x;
        float posY = position.min.y + EditorGUIUtility.singleLineHeight * 2;
        float width = position.size.x;
        float height = EditorGUIUtility.singleLineHeight;

        Rect drawArea = new Rect(posX, posY, width, height);
        EditorGUI.PropertyField(drawArea, valueProperty);
    }

    private void DrawBoolMaxStat(Rect position)
    {
        float posX = position.min.x;
        float posY = position.min.y + EditorGUIUtility.singleLineHeight * 3;
        float width = position.size.x;
        float height = EditorGUIUtility.singleLineHeight;

        Rect drawArea = new Rect(posX, posY, width, height);
        EditorGUI.PropertyField(drawArea, hasMaxStatProperty);
    }

    private void DrawMaxStat(Rect position)
    {
        float posX = position.min.x;
        float posY = position.min.y + EditorGUIUtility.singleLineHeight * 4;
        float width = position.size.x;
        float height = EditorGUIUtility.singleLineHeight;

        Rect drawArea = new Rect(posX, posY, width, height);
        EditorGUI.PropertyField(drawArea, maxStatProperty);
    }
}
#endif