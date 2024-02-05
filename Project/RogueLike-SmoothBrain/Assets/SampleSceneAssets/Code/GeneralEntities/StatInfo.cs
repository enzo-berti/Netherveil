using System;

// Si vous ajoutez un truc, ajoutez le à la fin
public enum Stat
{
    HP,
    ATK,
    ATK_COEFF,
    ATK_RANGE,
    FIRE_RATE,
    SPEED,
    CATCH_RADIUS,
    VISION_RANGE,
    CRIT_RATE,
    CRIT_DAMAGE,
    MAX_HP,
    CORRUPTION,
    LIFE_STEAL,
    HEAL_COEFF,
    KNOCKBACK_COEFF,
    STAGGER_DURATION
}

[Serializable]
public class StatInfo
{
    public Stat stat;
    public float value;
}