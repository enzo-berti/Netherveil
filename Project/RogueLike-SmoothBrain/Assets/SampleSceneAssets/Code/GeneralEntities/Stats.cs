using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stats
{
    [SerializeField] string name = "DefaultStats ( change name )";
    [SerializeField] List<StatInfo> stats = new();

    public int Size
    {
        get { return stats.Count; }
    }
    public List<Stat> GetStatsName
    {
        get
        {
            List<Stat> list = new();
            foreach(StatInfo info in stats)
            {
                list.Add(info.Stat);
            }
            return list;
        }
    }
    public float GetValueStat(Stat info)
    {
        foreach (StatInfo stat in stats)
        {
            if (stat.Stat == info)
            {
                return stat.Value;
            }
        }
        Debug.LogWarning($"Can't find {info} in {name}");
        return -1.0f;
    }

    public void IncreaseValue(Stat info, float increasingValue)
    {
        int index = stats.FindIndex(x => x.Stat == info);
        if (index != -1)
        {
            stats[index].Value += increasingValue;
        }
        else
            Debug.LogWarning($"Can't find {info} in {name}");
    }

    public bool HasStat(Stat info)
    {
        foreach(StatInfo stat in stats)
        {
            if (stat.Stat == info) return true;
        }
        return false;
    }

    //public StatInfo GetRandomStat()
    //{

    //}
}
