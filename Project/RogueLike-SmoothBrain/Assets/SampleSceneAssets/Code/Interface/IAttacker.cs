using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

public interface IAttacker 
{
    public List<Status> StatusToApply { get; }
    public delegate void AttackDelegate();
    public AttackDelegate OnAttack
    {
        get;
        set;
    }

    public delegate void HitDelegate(IDamageable damageable);
    public HitDelegate OnHit
    {
        get;
        set;
    }

    public void Attack(IDamageable damageable);

    public void ApplyStatus(IDamageable damageable)
    {
        Entity entity = damageable as Entity;
        if (entity == null) return;
        foreach (var status in StatusToApply)
        {
            entity.ApplyEffect(status.DeepCopy());
        }
    }
}
