using System.Collections;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class Electricity : OverTimeStatus
{
    private float entityBaseSpeed;
    const float stunTime = 0.5f;
    private bool isStunCoroutineOn = false;
    public Electricity(float duration, float chance) : base(duration, chance)
    {
        isStackable = false;
    }
    public override bool CanApplyEffect(Entity target)
    {
        return target.Stats.HasStat(Stat.SPEED);
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
        if (target != null && !isStunCoroutineOn)
        {
            Stun();
        }
    }

    private IEnumerator Stun()
    {
        isStunCoroutineOn = true;
        entityBaseSpeed = target.Stats.GetValue(Stat.SPEED);
        target.Stats.SetValue(Stat.SPEED, 0);
        yield return new WaitForSeconds(stunTime);
        target.Stats.SetValue(Stat.SPEED, entityBaseSpeed);
        isStunCoroutineOn = false;
    }

    protected override void PlayVFX()
    {
        PlayVfx("VFX_Electricity");
    }
}
