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

    #region Getters
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

    public float GeteMaxValueStat(Stat info)
    {
        foreach (StatInfo stat in stats)
        {
            if (stat.stat == info)
            {
                return stat.maxValue;
            }
        }

        Debug.LogWarning($"Can't find {info} in {name}");
        return -1.0f;
    }

    public bool HasStat(Stat info)
    {
        foreach (StatInfo stat in stats)
        {
            if (stat.stat == info) return true;
        }
        return false;
    }
    #endregion

    #region Maths
    public void IncreaseValue(Stat info, float increasingValue, bool clampToMaxValue)
    {
        int index = stats.FindIndex(x => x.stat == info);

        if (index != -1)
        {
            if(clampToMaxValue && stats[index].hasMaxStat)
                IncreaseValueClamp(info, increasingValue);

            else if(clampToMaxValue && !stats[index].hasMaxStat)
                Debug.LogWarning($"Missing max value of {info} in {name}");

            else
                stats[index].value += increasingValue;
        }
        else
        {
            Debug.LogWarning($"Can't find {info} in {name}");
        }
    }

    public void DecreaseValue(Stat info, float decreasingValue, bool clampToMinValue)
    {
        int index = stats.FindIndex(x => x.stat == info);

        if (index != -1)
        {
            if (clampToMinValue && stats[index].hasMinStat)
                DecreaseValueClamp(info, decreasingValue);

            else if (clampToMinValue && !stats[index].hasMinStat)
                Debug.LogWarning($"Missing min value of {info} in {name}");

            else
                stats[index].value -= decreasingValue;
        }
        else
            Debug.LogWarning($"Can't find {info} in {name}");
    }

    public void MultiplyValue(Stat info, float multiplyingValue, bool clampToMaxValue) 
    {
        int index = stats.FindIndex(x => x.stat == info);
        if (index != -1)
        {
            if (clampToMaxValue && stats[index].hasMaxStat)
                MultiplyValueClamp(info, multiplyingValue);

            else if (clampToMaxValue && !stats[index].hasMaxStat)
                Debug.LogWarning($"Missing min value of {info} in {name}");

            else
                stats[index].value *= multiplyingValue;
        }
        else
            Debug.LogWarning($"Can't find {info} in {name}");
    }

    public void DivideValue(Stat info, float dividingValue, bool clampToMinValue)
    {
        int index = stats.FindIndex(x => x.stat == info);
        if (index != -1)
        {
            if (clampToMinValue && stats[index].hasMinStat)
                DivideValueClamp(info, dividingValue);

            else if (clampToMinValue && !stats[index].hasMinStat)
                Debug.LogWarning($"Missing min value of {info} in {name}");

            else
                stats[index].value /= dividingValue;
        }
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
    #endregion

    #region ClampMaths
    private void IncreaseValueClamp(Stat statToIncrease, float increasingValue)
    {
        int indexIncrease = stats.FindIndex(x => x.stat == statToIncrease);

        if (indexIncrease != -1)
        {
            if(stats[indexIncrease].value + increasingValue > stats[indexIncrease].maxValue)
            {
                stats[indexIncrease].value = stats[indexIncrease].maxValue;
            }
            else
            {
                stats[indexIncrease].value += increasingValue;
            }
        }
           
        else if(indexIncrease == -1)
        {
            Debug.LogWarning($"Can't find {statToIncrease} in {name}");
        }
    }

    private void DecreaseValueClamp(Stat statToDecrease, float decreasingValue)
    {
        int indexDecrease = stats.FindIndex(x => x.stat == statToDecrease);

        if (indexDecrease != -1)
        {
            if (stats[indexDecrease].value - decreasingValue < stats[indexDecrease].minValue)
            {
                stats[indexDecrease].value = stats[indexDecrease].minValue;
            }
            else
            {
                stats[indexDecrease].value -= decreasingValue;
            }
        }

        else if (indexDecrease == -1)
        {
            Debug.LogWarning($"Can't find {statToDecrease} in {name}");
        }
    }

    private void MultiplyValueClamp(Stat statToIncrease, float increasingValue)
    {
        int indexIncrease = stats.FindIndex(x => x.stat == statToIncrease);

        if (indexIncrease != -1)
        {
            if (stats[indexIncrease].value * increasingValue > stats[indexIncrease].maxValue)
            {
                stats[indexIncrease].value = stats[indexIncrease].maxValue;
            }
            else
            {
                stats[indexIncrease].value *= increasingValue;
            }
        }

        else if (indexIncrease == -1)
        {
            Debug.LogWarning($"Can't find {statToIncrease} in {name}");
        }
    }

    private void DivideValueClamp(Stat statToDecrease, float decreasingValue)
    {
        int indexDecrease = stats.FindIndex(x => x.stat == statToDecrease);

        if (indexDecrease != -1)
        {
            if (stats[indexDecrease].value / decreasingValue < stats[indexDecrease].minValue)
            {
                stats[indexDecrease].value = stats[indexDecrease].minValue;
            }
            else
            {
                stats[indexDecrease].value /= decreasingValue;
            }
        }

        else if (indexDecrease == -1)
        {
            Debug.LogWarning($"Can't find {statToDecrease} in {name}");
        }
    }
    #endregion
   
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