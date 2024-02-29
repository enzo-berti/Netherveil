using System;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using TMPro.EditorUtilities;

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

    public bool hasMaxStat;
    public bool hasMinStat;

    public float value;
    public float maxValue;
    public float minValue;
}


#if UNITY_EDITOR
///// Affichage carré dans l'éditeur /////

[CustomPropertyDrawer(typeof(StatInfo))]
public class StatInfoDrawerUIE : PropertyDrawer
{
    SerializedProperty statProperty;
    SerializedProperty valueProperty;
    SerializedProperty hasMaxStatProperty;
    SerializedProperty hasMinStatProperty;
    SerializedProperty maxStatProperty;
    SerializedProperty minStatProperty;

    int nbMember = 0;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        nbMember = 0;
        EditorGUI.BeginProperty(position, label, property);
        statProperty = property.FindPropertyRelative("stat");
        valueProperty = property.FindPropertyRelative("value");
        hasMaxStatProperty = property.FindPropertyRelative("hasMaxStat");
        hasMinStatProperty = property.FindPropertyRelative("hasMinStat");

        maxStatProperty = property.FindPropertyRelative("maxValue");
        minStatProperty = property.FindPropertyRelative("minValue");

        Rect foldoutBox = new Rect(position.min.x, position.min.y, position.size.x, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(foldoutBox, property.isExpanded, label);
        if (property.isExpanded)
        {
            DrawMember(position, statProperty);
            DrawMember(position, valueProperty);
            DrawMember(position, hasMaxStatProperty);
            if (hasMaxStatProperty.boolValue) DrawMember(position, maxStatProperty);
            DrawMember(position, hasMinStatProperty);   
            if (hasMinStatProperty.boolValue) DrawMember(position, minStatProperty);

        }
        EditorGUI.EndProperty();
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int totalLine = 1;
        if (property.isExpanded)
        {
            totalLine += 4;
            if (property.FindPropertyRelative("hasMaxStat").boolValue) totalLine++;
            if (property.FindPropertyRelative("hasMinStat").boolValue) totalLine++;
            
        }
        return EditorGUIUtility.singleLineHeight * totalLine;
    }

    private void DrawMember(Rect position, SerializedProperty propertyToDraw)
    {
        nbMember++;
        float posX = position.min.x + 0.5f;
        float posY = position.min.y + EditorGUIUtility.singleLineHeight * nbMember;
        float width = position.size.x;
        float height = EditorGUIUtility.singleLineHeight;

        Rect drawArea = new Rect(posX, posY, width, height);
        EditorGUI.PropertyField(drawArea, propertyToDraw);
    }
    private void DrawStat(Rect position)
    {
        nbMember++;
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

    private void DrawBoolMinStat(Rect position)
    {
        float posX = position.min.x;
        float posY = position.min.y + EditorGUIUtility.singleLineHeight * 4;
        float width = position.size.x;
        float height = EditorGUIUtility.singleLineHeight;

        Rect drawArea = new Rect(posX, posY, width, height);
        EditorGUI.PropertyField(drawArea, hasMinStatProperty);
    }

    private void DrawMaxStat(Rect position)
    {
        float posX = position.min.x;
        float posY = position.height + EditorGUIUtility.singleLineHeight * 5;
        float width = position.size.x;
        float height = EditorGUIUtility.singleLineHeight;

        Rect drawArea = new Rect(posX, posY, width, height);
        EditorGUI.PropertyField(drawArea, maxStatProperty);
    }
    private void DrawMinStat(Rect position)
    {
        float posX = position.min.x;
        float posY = position.min.y + EditorGUIUtility.singleLineHeight * 6;
        float width = position.size.x;
        float height = EditorGUIUtility.singleLineHeight;

        Rect drawArea = new Rect(posX, posY, width, height);
        EditorGUI.PropertyField(drawArea, minStatProperty);
    }
}
#endif