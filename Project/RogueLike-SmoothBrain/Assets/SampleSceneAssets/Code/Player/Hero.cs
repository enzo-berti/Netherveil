using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : EntityUnit, IDamageable
{
    public void GetDamage(int _value)
    {
        this.Stats.IncreaseValue(Stat.HP, _value);
    }
}
