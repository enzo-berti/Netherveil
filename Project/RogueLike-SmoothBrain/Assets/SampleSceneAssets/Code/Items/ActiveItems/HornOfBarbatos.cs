using Map;
using System.Collections.Generic;
using UnityEngine;

public class HornOfBarbatos : ItemEffect, IActiveItem
{
    public float Cooldown { get; set; } = 35f;
    private float increaseValue = 0.2f;
#pragma warning disable IDE0052 // Supprimer les membres privés non lus
    private readonly float displayValue;
#pragma warning restore IDE0052 // Supprimer les membres privés non lus
    List<float> changesList = new List<float>();
    private List<Stat> statToChange = new List<Stat>()
    {
        Stat.ATK,
        Stat.SPEED,
        Stat.ATK_RANGE
    };

    bool itemActivatedThisRoom = false;

    public HornOfBarbatos()
    {
        displayValue = Cooldown;
        MapUtilities.onExit += ResetStat;
    }

    ~HornOfBarbatos()
    {
        MapUtilities.onExit -= ResetStat;
    }

    public void Activate()
    {
        Camera.main.GetComponent<CameraUtilities>().ShakeCamera(0.3f, 0.25f, EasingFunctions.EaseInQuint);
        AudioManager.Instance.PlaySound(AudioManager.Instance.HornOfBarbatosSFX);
        //add sfx here

        Hero hero = Utilities.Hero;
        foreach (var stat in hero.Stats.StatsName)
        {
            if (statToChange.Contains(stat))
            {
                float change = hero.Stats.GetCoeff(stat);
                change = change * 0.2f;
                changesList.Add(change);
                hero.Stats.IncreaseCoeffValue(stat, change);
            }
        }
        itemActivatedThisRoom = true;
    }

    private void ResetStat()
    {
        if (itemActivatedThisRoom)
        {
            Hero hero = Utilities.Hero;
            int i = 0;
            foreach (var stat in hero.Stats.StatsName)
            {
                if (statToChange.Contains(stat))
                {
                    hero.Stats.DecreaseCoeffValue(stat, changesList[i]);
                    i++;
                }
            }
        }
        changesList.Clear();
        itemActivatedThisRoom = false;
    }
}
