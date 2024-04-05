using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class Electricity : Status
{
    private float entityBaseSpeed;
    const float stunTime = 0.2f;

    public Electricity(float duration, float chance) : base(duration, chance)
    {
        isStackable = false;
        frequency = 1f;
    }
    public override void ApplyEffect(Entity target, IAttacker attacker)
    {
        Debug.Log("apply effect");
        if (target.Stats.HasStat(Stat.SPEED))
        {
            launcher = attacker;
            target.AddStatus(this);
            entityBaseSpeed = target.Stats.GetValue(Stat.SPEED);
            PlayVfx("VFX_Electricity");
        }
    }

    public override Status DeepCopy()
    {
        Electricity electricity = (Electricity)MemberwiseClone();
        return electricity;
    }

    public override void OnFinished()
    {
        target.Stats.SetValue(Stat.SPEED, entityBaseSpeed);
    }

    protected override void Effect()
    {
        if (target != null)
        {
            Stun();
        }
    }

    private async void Stun()
    {
        target.Stats.SetValue(Stat.SPEED, 0);
        await Task.Delay((int)(stunTime * 1000));
        target.Stats.SetValue(Stat.SPEED, entityBaseSpeed);
    }
}
