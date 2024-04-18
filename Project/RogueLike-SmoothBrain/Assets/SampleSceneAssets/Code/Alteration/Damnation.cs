using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damnation : Status
{
    public Damnation(float _duration, float _chance) : base(_duration, _chance)
    {
        isStackable = false;
    }

    public override void ApplyEffect(Entity target, IAttacker attacker)
    {
        target.GetComponent<Mobs>().DamageTakenMultiplicator += 1.0f;
    }

    public override Status DeepCopy()
    {
        Damnation status = (Damnation)this.MemberwiseClone();
        return status;
    }

    public override void OnFinished()
    {
        target.GetComponent<Mobs>().DamageTakenMultiplicator -= 1.0f;
    }

    protected override void Effect()
    {
        return;
    }
}
