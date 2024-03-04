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
    // Get number of stats
    public int Size
    {
        get { return stats.Count; }
    }

    public string GetEntityName()
    {
        return name;
    }

    // Return a list with all stats used. Exemple return {ATK, HP, CATCH_RANGE}
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
    // Get value of a stat, if there is a coeff, returns value * coeff
    public float GetValue(Stat info)
    {
        foreach (StatInfo stat in stats)
        {
            if (stat.stat == info)
            {
                float coeff = stat.hasCoeff ? stat.coeff : 1;
                return stat.value * coeff;
            }
        }

        Debug.LogWarning($"Can't find {info} in {name}");
        return -1.0f;
    }

    // Returns straight value
    public float GetValueWithoutCoeff(Stat info)
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

    // Get the maximum value of a stat
    public float GetMaxValue(Stat info)
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

    // Get the minimal value of a stat
    public float GetMinValue(Stat info)
    {
        foreach (StatInfo stat in stats)
        {
            if (stat.stat == info)
            {
                return stat.minValue;
            }
        }

        Debug.LogWarning($"Can't find {info} in {name}");
        return -1.0f;
    }

    // If has coeff, returns coeff. Else, returns 1
    public float GetCoeff(Stat info)
    {
        foreach (StatInfo stat in stats)
        {
            if (stat.stat == info)
            {
                if(stat.hasCoeff)
                    return stat.coeff;
                else
                    return 1.0f;
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
        float baseValue = stats[index].value;

        if (index != -1)
        {
            if (clampToMaxValue && stats[index].hasMaxStat)
                IncreaseValueClamp(info, increasingValue);

            else if (!clampToMaxValue && stats[index].hasMaxStat)
                IncreaseValueOverload(info, increasingValue);

            else if (clampToMaxValue && !stats[index].hasMaxStat)
            {
                Debug.LogWarning($"Missing max value of {info} in {name}");
                return;
            }
               

            else
            {
                if (stats[index].underload > 0)
                {
                    float realIncrease = increasingValue - stats[index].underload;
                    stats[index].underload -= increasingValue;
                    if (realIncrease > 0.0f)
                    {
                        stats[index].underload = 0;
                        stats[index].value += realIncrease;
                    }
                }
                    
                else
                    stats[index].value += increasingValue;
            }
            if (stats[index].value != baseValue) stats[index].onStatChange?.Invoke(info);
        }
        else
        {
            Debug.LogWarning($"Can't find {info} in {name}");
        }
    }

    public void IncreaseMaxValue(Stat info, float increasingValue)
    {
        int index = stats.FindIndex(x => x.stat == info);

        if (index != -1)
        {
            if (stats[index].hasMaxStat)
                stats[index].maxValue += increasingValue;
            else
                Debug.LogWarning($"Missing max value of {info} in {name}");
        }
        else
        {
            Debug.LogWarning($"Can't find {info} in {name}");
        }
    }

    public void IncreaseMinValue(Stat info, float increasingValue)
    {
        int index = stats.FindIndex(x => x.stat == info);

        if (index != -1)
        {
            if (stats[index].hasMinStat)
                stats[index].minValue += increasingValue;

            else
                Debug.LogWarning($"Missing min value of {info} in {name}");
        }
        else
        {
            Debug.LogWarning($"Can't find {info} in {name}");
        }
    }

    public void DecreaseValue(Stat info, float decreasingValue, bool clampToMinValue)
    {
        int index = stats.FindIndex(x => x.stat == info);
        float baseValue = stats[index].value;
        if (index != -1)
        {
            if (clampToMinValue && stats[index].hasMinStat)
                DecreaseValueClamp(info, decreasingValue);

            else if (!clampToMinValue && stats[index].hasMaxStat)
                DecreaseValueUnderload(info, decreasingValue);

            else if (clampToMinValue && !stats[index].hasMinStat)
            {
                Debug.LogWarning($"Missing min value of {info} in {name}");
                return;
            }
                

            else
                stats[index].value -= decreasingValue;

            if (baseValue != stats[index].value) stats[index].onStatChange?.Invoke(info);
        }
        else
            Debug.LogWarning($"Can't find {info} in {name}");
    }

    public void DecreaseMaxValue(Stat info, float decreasingValue)
    {
        int index = stats.FindIndex(x => x.stat == info);

        if (index != -1)
        {
            if (stats[index].hasMaxStat)
                stats[index].maxValue -= decreasingValue;
            else
                Debug.LogWarning($"Missing max value of {info} in {name}");
        }
        else
        {
            Debug.LogWarning($"Can't find {info} in {name}");
        }
    }

    public void DecreaseMinValue(Stat info, float decreasingValue)
    {
        int index = stats.FindIndex(x => x.stat == info);

        if (index != -1)
        {
            if (stats[index].hasMinStat)
                stats[index].minValue -= decreasingValue;

            else
                Debug.LogWarning($"Missing min value of {info} in {name}");
        }
        else
        {
            Debug.LogWarning($"Can't find {info} in {name}");
        }
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

    public void MultiplyMaxValue(Stat info, float multiplyingValue)
    {
        int index = stats.FindIndex(x => x.stat == info);

        if (index != -1)
        {
            if (stats[index].hasMaxStat)
                stats[index].maxValue *= multiplyingValue;
            else
                Debug.LogWarning($"Missing max value of {info} in {name}");
        }
        else
        {
            Debug.LogWarning($"Can't find {info} in {name}");
        }
    }

    public void MultiplyMinValue(Stat info, float multiplyingValue)
    {
        int index = stats.FindIndex(x => x.stat == info);

        if (index != -1)
        {
            if (stats[index].hasMinStat)
                stats[index].minValue *= multiplyingValue;

            else
                Debug.LogWarning($"Missing min value of {info} in {name}");
        }
        else
        {
            Debug.LogWarning($"Can't find {info} in {name}");
        }
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

    public void DivideMaxValue(Stat info, float dividingValue)
    {
        int index = stats.FindIndex(x => x.stat == info);

        if (index != -1)
        {
            if (stats[index].hasMaxStat)
                stats[index].maxValue /= dividingValue;
            else
                Debug.LogWarning($"Missing max value of {info} in {name}");
        }
        else
        {
            Debug.LogWarning($"Can't find {info} in {name}");
        }
    }

    public void DivideMinValue(Stat info, float dividingValue)
    {
        int index = stats.FindIndex(x => x.stat == info);

        if (index != -1)
        {
            if (stats[index].hasMinStat)
                stats[index].minValue /= dividingValue;

            else
                Debug.LogWarning($"Missing min value of {info} in {name}");
        }
        else
        {
            Debug.LogWarning($"Can't find {info} in {name}");
        }
    }

    public void SetValue(Stat info, float value)
    {
        int index = stats.FindIndex(x => x.stat == info);
        if (index != -1)
            stats[index].value = value;
        else
            Debug.LogWarning($"Can't find {info} in {name}");
    }

    public void SetMaxValue(Stat info, float value)
    {
        int index = stats.FindIndex(x => x.stat == info);
        if (index != -1)
        {
            if (stats[index].hasMaxStat)
                stats[index].maxValue = value;
            else
                Debug.Log($"Missing max value of {info} in {name}");
        }
            
        else
            Debug.LogWarning($"Can't find {info} in {name}");
    }

    public void SetMinValue(Stat info, float value)
    {
        int index = stats.FindIndex(x => x.stat == info);
        if (index != -1)
        {
            if (stats[index].hasMinStat)
                stats[index].minValue = value;
            else
                Debug.Log($"Missing min value of {info} in {name}");
        }

        else
            Debug.LogWarning($"Can't find {info} in {name}");
    }

    public void SetCoeffValue(Stat info, float value)
    {
        int index = stats.FindIndex(x => x.stat == info);
        if (index != -1)
        {
            if (stats[index].hasCoeff)
                stats[index].coeff = value;
            else
                Debug.Log($"Missing coeff value of {info} in {name}");
        }

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

    #region Overloard/Underloard
    private void IncreaseValueOverload(Stat statToIncrease, float increasingValue)
    {
        int indexIncrease = stats.FindIndex(x => x.stat == statToIncrease);

        if (indexIncrease != -1)
        {
            float realIncrease = increasingValue;
            if (stats[indexIncrease].underload > 0)
            {
                realIncrease -= stats[indexIncrease].underload;
                stats[indexIncrease].underload -= increasingValue;
            }
            if(realIncrease > 0f)
            {
                stats[indexIncrease].underload = 0;
                if (stats[indexIncrease].value + realIncrease > stats[indexIncrease].maxValue)
                {
                    float overloardValue = stats[indexIncrease].value + realIncrease - stats[indexIncrease].maxValue;
                    stats[indexIncrease].value = stats[indexIncrease].maxValue;
                    stats[indexIncrease].overload += overloardValue;
                }
                else
                {
                    stats[indexIncrease].value += realIncrease;
                }
            }
            
        }

        else if (indexIncrease == -1)
        {
            Debug.LogWarning($"Can't find {statToIncrease} in {name}");
        }
    }

    private void DecreaseValueUnderload(Stat statToDecrease, float decreasingValue)
    {
        int indexDecrease = stats.FindIndex(x => x.stat == statToDecrease);

        if (indexDecrease != -1)
        {
            float realDecrease = decreasingValue;
            if (stats[indexDecrease].underload > 0)
            {
                realDecrease -= stats[indexDecrease].underload;
                stats[indexDecrease].underload -= realDecrease;
            }
            if (realDecrease > 0f)
            {
                stats[indexDecrease].underload = 0;
                if (stats[indexDecrease].value - realDecrease < stats[indexDecrease].minValue)
                {
                    float underloadValue = realDecrease - stats[indexDecrease].value + stats[indexDecrease].minValue;
                    stats[indexDecrease].value = stats[indexDecrease].minValue;
                    stats[indexDecrease].underload += underloadValue;
                }
                else
                {
                    stats[indexDecrease].value -= realDecrease;
                }
            }

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