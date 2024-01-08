using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mobs : Entity, IDamageable
{
    public void ApplyDamage(int _value)
    {
        Stats.IncreaseValue(Stat.HP, _value);
    }
}
