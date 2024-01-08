using System;

public enum Stat
{
    HP,
    ATK,
    ATK_COEFF,
    ATK_RANGE,
    FIRE_RATE,
    SPEED,
    CATCH_RADIUS
}

[Serializable]
public class StatInfo
{
    public Stat stat;
    public float value;
}