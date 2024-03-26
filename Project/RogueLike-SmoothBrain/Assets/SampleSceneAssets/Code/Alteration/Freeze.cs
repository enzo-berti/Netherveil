using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freeze : Status
{
    public Freeze(float duration) : base(duration)
    {
        isConst = true;
    }

    public override void ApplyEffect(Entity target)
    {
        if (target.Stats.HasStat(Stat.SPEED))
            target.AddStatus(this);
    }

    public override Status DeepCopy()
    {
        throw new System.NotImplementedException();
    }

    public override void OnFinished()
    {
        throw new System.NotImplementedException();
    }


    protected override void Effect()
    {
        throw new System.NotImplementedException();
    }
}
