using System;

public enum Stat
{
    HP,
    ATK,
    ATK_COEFF,
    ATK_RANGE,
    FIRE_RATE,
    SPEED,
    CATCH_RADIUS,
    CRIT_RATE,
    CRIT_DAMAGE,
    MAX_HP,
    CORRUPTION,
    LIFE_STEAL
}

[Serializable]
public class StatInfo
{
    public Stat stat;
    public float value;
}