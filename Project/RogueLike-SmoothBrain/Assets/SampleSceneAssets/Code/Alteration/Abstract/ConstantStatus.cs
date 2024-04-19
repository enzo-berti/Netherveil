using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConstantStatus : Status
{
    protected ConstantStatus(float _duration, float _chance) : base(_duration, _chance)
    {
    }

    public sealed override void ApplyEffect(Entity target)
    {
        base.ApplyEffect(target);
        OnAddStack += DoEffect;
    }

    public override void DoEffect()
    {
        Effect();
    }

}
