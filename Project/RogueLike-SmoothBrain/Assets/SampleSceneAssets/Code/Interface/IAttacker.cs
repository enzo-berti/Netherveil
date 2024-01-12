using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttacker 
{
    public delegate void AttackDelegate();
    public delegate void HitDelegate();
    public AttackDelegate OnAttack
    {
        get;
        set;
    }

    public AttackDelegate OnHit
    {
        get;
        set;
    }
}
