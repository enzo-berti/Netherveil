using System;

public enum Stat
{
    HP,
    ATK,
    ATK_COEFF,
    FIRE_RATE,
    SPEED,
    CATCH_RADIUS
}
[Serializable]
public class StatInfo
{
    public Stat Stat;
    public float Value;
}
