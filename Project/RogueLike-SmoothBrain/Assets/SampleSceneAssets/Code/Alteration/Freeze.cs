using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freeze : Status
{ 
    public override void ApplyEffect(Entity target)
    {
        if (target.Stats.HasStat(Stat.SPEED))
            target.AddStatus(this);
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
