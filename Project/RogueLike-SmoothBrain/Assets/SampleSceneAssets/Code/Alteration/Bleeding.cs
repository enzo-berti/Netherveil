using System.Collections;
using UnityEngine;

public class Bleeding : OverTimeStatus
{
    readonly float coefValue = 0.03f;
    static Color bleedingColor = new(0.5f, 0.11f, 0.11f, 1f);

    public Bleeding(float _duration, float _chance) : base(_duration, _chance)
    {
        isStackable = true;
        maxStack = 3;
        
    }
    public override Status DeepCopy()
    {
        Bleeding bleeding = (Bleeding)this.MemberwiseClone();
        return bleeding;
    }

    public override void OnFinished()
    {
        throw new System.NotImplementedException();
    }

    protected override void Effect()
    {
        if (target != null)
        {
            int damages = (int)(target.Stats.GetMaxValue(Stat.HP) * coefValue * Stack);
            FloatingTextGenerator.CreateEffectDamageText(damages, target.transform.position, bleedingColor);
            target.gameObject.GetComponent<IDamageable>().ApplyDamage(damages, launcher, false);
        }
    }

    public override bool CanApplyEffect(Entity target)
    {
        return target.gameObject.TryGetComponent<IDamageable>(out _);
    }

    protected override void PlayVFX()
    {
        PlayVfx("VFX_Fire");
    }
}
