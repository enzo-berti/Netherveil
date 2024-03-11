using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freeze : Status
{
    public Freeze(Entity launcher, float duration) : base(launcher, duration)
    {
        isConst = true;
    }

    public override void ApplyEffect(Entity target)
    {
        if (target.Stats.HasStat(Stat.SPEED))
            target.AddStatus(this);
    }

    public override void OnFinished()
    {
        throw new System.NotImplementedException();
    }

    public override Status ShallowCopy()
    {
        return (Freeze)this.MemberwiseClone();
    }

    protected override void Effect()
    {
        throw new System.NotImplementedException();
    }
}
