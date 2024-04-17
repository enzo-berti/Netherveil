using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class Freeze : Status
{
    float baseAgentSpeed;
    public Freeze(float duration, float chance) : base(duration, chance)
    {
        isStackable = false;
    }

    public override void ApplyEffect(Entity target, IAttacker attacker)
    {
        Debug.Log("Freeze " + target.name);
        if (target.Stats.HasStat(Stat.SPEED))
        {
            baseAgentSpeed = target.Stats.GetCoeff(Stat.SPEED);
            target.AddStatus(this);
            PlayVfx("VFX_Frozen");
        }
    }

    public override Status DeepCopy()
    {
        Freeze freeze = (Freeze)MemberwiseClone();
        return freeze;
    }

    protected override void Effect()
    {
        if(target != null)
        {
            target.Stats.SetValue(Stat.SPEED, 0);
        }
    }

    public override void OnFinished()
    {
        target.Stats.SetValue(Stat.SPEED, baseAgentSpeed);
    }
}
