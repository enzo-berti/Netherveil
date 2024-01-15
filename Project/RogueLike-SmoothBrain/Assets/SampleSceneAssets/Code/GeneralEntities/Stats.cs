using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

[Serializable]
public class Stats
{ 
    [SerializeField] List<StatInfo> stats = new();
    [SerializeField] private string name = "Default";

    public int Size
    {
        get { return stats.Count; }
    }

    public List<Stat> StatsName
    {
        get
        {
            List<Stat> list = new();
            foreach (StatInfo info in stats)
            {
                list.Add(info.stat);
            }

            return list;
        }
    }
    public float GetValueStat(Stat info)
    {
        foreach (StatInfo stat in stats)
        {
            if (stat.stat == info)
            {
                return stat.value;
            }
        }

        Debug.LogWarning($"Can't find {info} in {name}");
        return -1.0f;
    }   

    public void IncreaseValue(Stat info, float increasingValue)
    {
        int index = stats.FindIndex(x => x.stat == info);

        if (index != -1)
            stats[index].value += increasingValue;
        else
            Debug.LogWarning($"Can't find {info} in {name}");
    }

    public bool HasStat(Stat info)
    {
        foreach (StatInfo stat in stats)
        {
            if (stat.stat == info) return true;
        }
        return false;
    }
}

#if UNITY_EDITOR
///// Affichage carr� dans l'�diteur /////

[CustomPropertyDrawer(typeof(Stats))]
public class StatsDrawerUIE : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Create property container element.
        var container = new VisualElement();

        // Create property fields.
        var nameField = new PropertyField(property.FindPropertyRelative("name"), "Name");
        var statsField = new PropertyField(property.FindPropertyRelative("stats"));

        // Add fields to the container.
        container.Add(nameField);
        container.Add(statsField);

        return container;
    }
}
#endif