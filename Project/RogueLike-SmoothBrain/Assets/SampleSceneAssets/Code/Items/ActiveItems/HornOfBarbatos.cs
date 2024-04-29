using Map;
using System.Collections.Generic;
using UnityEngine;

public class HornOfBarbatos : ItemEffect, IActiveItem
{
    public float Cooldown { get; set; } = 10f;
    private float increaseValue = 0.2f;
#pragma warning disable IDE0052 // Supprimer les membres privés non lus
    private readonly float displayValue;
#pragma warning restore IDE0052 // Supprimer les membres privés non lus
    private List<Stat> avoidedStat = new List<Stat>()
    {
        Stat.CORRUPTION,
        Stat.ATK_RANGE,
        Stat.HP
    };

    public HornOfBarbatos()
    {
        displayValue = Cooldown;
        RoomUtilities.exitEvents += ResetStat;
    }

    ~HornOfBarbatos()
    {
        RoomUtilities.exitEvents -= ResetStat;
    }

    public void Activate()
    {
        Camera.main.GetComponent<CameraUtilities>().ShakeCamera(0.3f, 0.25f, EasingFunctions.EaseInQuint);
        AudioManager.Instance.PlaySound(AudioManager.Instance.HornOfBarbatosSFX);
        //add sfx here

        Hero hero = Utilities.Hero;
        foreach (var stat in hero.Stats.StatsName)
        {
            if (!avoidedStat.Contains(stat))
            {
                hero.Stats.MultiplyCoeffValue(stat, 1 + increaseValue);
            }
        }
    }

    private void ResetStat()
    {
        Hero hero = Utilities.Hero;
        foreach (var stat in hero.Stats.StatsName)
        {
            if (!avoidedStat.Contains(stat))
            {
                hero.Stats.DivideCoeffValue(stat, 1 + increaseValue);
            }
        }
    }
}
