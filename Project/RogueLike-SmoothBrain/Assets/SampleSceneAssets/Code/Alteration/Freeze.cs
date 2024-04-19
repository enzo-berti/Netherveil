using UnityEngine;

public class Freeze : ConstantStatus
{
    float baseAgentSpeed;
    public Freeze(float _duration, float _chance) : base(_duration, _chance)
    {
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

    public override bool CanApplyEffect(Entity target)
    {
        return target.Stats.HasStat(Stat.SPEED);
    }

    protected override void PlayVFX()
    {
        PlayVfx("VFX_Frozen");
    }
}
