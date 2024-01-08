using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Entity, IDamageable
{
    public void ApplyDamage(int _value)
    {
        this.Stats.IncreaseValue(Stat.HP, _value);
    }
}
