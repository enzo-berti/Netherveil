using PostProcessingEffects;
using System.Collections;
using UnityEngine;

public class Electricity : OverTimeStatus
{
    private float entityBaseSpeed;
    const float stunTime = 0.5f;
    private bool isStunCoroutineOn = false;
    public Electricity(float duration, float chance) : base(duration, chance)
    {
        isStackable = false;
        frequency = 1.0f;
        vfxName = "VFX_Electricity";
        
    }
    public override bool CanApplyEffect(Entity target)
    {
        return target.Stats.HasStat(Stat.SPEED);
    }

    public override Status DeepCopy()
    {
        Electricity electricity = (Electricity)MemberwiseClone();
        electricity.stopTimes = new();
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
            CoroutineManager.Instance.StartCustom(Stun());

            if (Utilities.IsPlayer(target))
                PostProcessingEffectManager.current.Play(PostProcessingEffects.Effect.Electricity);
        }
    }

    private IEnumerator Stun()
    {
        isStunCoroutineOn = true;
        entityBaseSpeed = target.Stats.GetValue(Stat.SPEED);
        target.Stats.SetValue(Stat.SPEED, 0);
        target.GetComponentInChildren<Animator>().speed = 0;
        yield return new WaitForSeconds(stunTime);
        target.GetComponentInChildren<Animator>().speed = 1;
        target.Stats.SetValue(Stat.SPEED, entityBaseSpeed);
        isStunCoroutineOn = false;
    }

    protected override void PlayStatus()
    {
        entityBaseSpeed = target.Stats.GetValue(Stat.SPEED);
    }
}
