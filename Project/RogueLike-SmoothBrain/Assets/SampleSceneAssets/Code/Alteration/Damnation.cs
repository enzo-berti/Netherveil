using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damnation : ConstantStatus
{
    public Damnation(float _duration, float _chance) : base(_duration, _chance)
    {
        isStackable = false;
    }

    public override bool CanApplyEffect(Entity target)
    {
        return target.TryGetComponent<Mobs>(out _);
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
        target.GetComponent<Mobs>().DamageTakenMultiplicator += 1.0f;
    }

    protected override void PlayVFX()
    {
        throw new System.NotImplementedException("Bleeding VFX is not implemented");
    }
}
