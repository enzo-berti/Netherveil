using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public enum Stat
{
    LVL,
    HP,
    ATK,
    DEFENSE,
    FIRE_RATE,
    XP,
    SPEED,
    CATCH_RADIUS
}
[Serializable]
public class StatInfo
{
    public Stat Stat;
    public float Value;
}
