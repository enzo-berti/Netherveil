using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;
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

    public string GetEntityName()
    {
        return name;
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

    public void DecreaseValue(Stat info, float decreasingValue)
    {
        int index = stats.FindIndex(x => x.stat == info);

        if (index != -1)
            stats[index].value -= decreasingValue;
        else
            Debug.LogWarning($"Can't find {info} in {name}");
    }

    public void MultiplyValue(Stat info, float multiplyingValue) 
    {
        int index = stats.FindIndex(x => x.stat == info);

        if (index != -1)
            stats[index].value *= multiplyingValue;
        else
            Debug.LogWarning($"Can't find {info} in {name}");
    }

    public void DivideValue(Stat info, float dividingValue)
    {
        int index = stats.FindIndex(x => x.stat == info);

        if (index != -1)
            stats[index].value /= dividingValue;
        else
            Debug.LogWarning($"Can't find {info} in {name}");
    }

    public void SetValue(Stat info, float value)
    {
        int index = stats.FindIndex(x => x.stat == info);

        if (index != -1)
            stats[index].value = value;
        else
            Debug.LogWarning($"Can't find {info} in {name}");
    }

    public void IncreaseValueClamp(Stat statToIncrease, Stat maxStat, float increasingValue, float maxValue)
    {
        int indexIncrease = stats.FindIndex(x => x.stat == statToIncrease);
        int indexMaxStat = stats.FindIndex(x => x.stat == maxStat);

        if (indexIncrease != -1 && indexMaxStat != -1)
        {
            if(stats[indexIncrease].value + indexIncrease > stats[indexMaxStat].value)
            {
                stats[indexIncrease].value = stats[indexMaxStat].value;
            }
            else
            {
                stats[indexIncrease].value += indexIncrease;
            }
        }
           
        else if(indexIncrease != -1)
        {
            Debug.LogWarning($"Can't find {statToIncrease} in {name}");
        }
        else
        {
            Debug.LogWarning($"Can't find {maxStat} in {name}");
        }
    }

    public void DecreaseValueClamp(Stat statToDecrease, Stat minStat, float decreasingValue, float minValue)
    {
        int indexDecrease = stats.FindIndex(x => x.stat == statToDecrease);
        int indexMinValue = stats.FindIndex(x => x.stat == minStat);

        if (indexDecrease != -1 && indexMinValue != -1)
        {
            if (stats[indexDecrease].value - indexDecrease < stats[indexMinValue].value)
            {
                stats[indexDecrease].value = stats[indexMinValue].value;
            }
            else
            {
                stats[indexDecrease].value -= indexDecrease;
            }
        }

        else if (indexDecrease != -1)
        {
            Debug.LogWarning($"Can't find {statToDecrease} in {name}");
        }
        else
        {
            Debug.LogWarning($"Can't find {minStat} in {name}");
        }
    }

    public void MultiplyValueClamp(Stat statToIncrease, Stat maxStat, float increasingValue, float maxValue)
    {
        int indexIncrease = stats.FindIndex(x => x.stat == statToIncrease);
        int indexMaxStat = stats.FindIndex(x => x.stat == maxStat);

        if (indexIncrease != -1 && indexMaxStat != -1)
        {
            if (stats[indexIncrease].value * indexIncrease > stats[indexMaxStat].value)
            {
                stats[indexIncrease].value = stats[indexMaxStat].value;
            }
            else
            {
                stats[indexIncrease].value *= indexIncrease;
            }
        }

        else if (indexIncrease != -1)
        {
            Debug.LogWarning($"Can't find {statToIncrease} in {name}");
        }
        else
        {
            Debug.LogWarning($"Can't find {maxStat} in {name}");
        }
    }

    public void DivideValueClamp(Stat statToDecrease, Stat minStat, float decreasingValue, float minValue)
    {
        int indexDecrease = stats.FindIndex(x => x.stat == statToDecrease);
        int indexMinValue = stats.FindIndex(x => x.stat == minStat);

        if (indexDecrease != -1 && indexMinValue != -1)
        {
            if (stats[indexDecrease].value / indexDecrease < stats[indexMinValue].value)
            {
                stats[indexDecrease].value = stats[indexMinValue].value;
            }
            else
            {
                stats[indexDecrease].value /= indexDecrease;
            }
        }

        else if (indexDecrease != -1)
        {
            Debug.LogWarning($"Can't find {statToDecrease} in {name}");
        }
        else
        {
            Debug.LogWarning($"Can't find {minStat} in {name}");
        }
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
///// Affichage carré dans l'éditeur /////

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