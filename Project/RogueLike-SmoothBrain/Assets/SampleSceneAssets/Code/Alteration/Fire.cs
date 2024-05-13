using PostProcessingEffects;
using System;
using UnityEngine;

public class Fire : OverTimeStatus
{
    private int damage = 6;
    static Color fireColor = new Color(0.929f, 0.39f, 0.08f, 1.0f);
    public static event Action OnFire;
    public Fire(float _duration, float _statusChance) : base(_duration, _statusChance)
    {
        frequency = 0.5f;
        isStackable = false;
        vfxName = "VFX_Fire";
    }
    protected override void Effect()
    {
        if (target != null)
        {
            FloatingTextGenerator.CreateEffectDamageText(damage, target.transform.position, fireColor);
            target.gameObject.GetComponent<IDamageable>().ApplyDamage(damage, launcher, false);

            if (Utilities.IsPlayer(target))
                PostProcessingEffectManager.current.Play(PostProcessingEffects.Effect.Fire);
        }
    }

    public override Status DeepCopy()
    {
        Fire fire = (Fire)this.MemberwiseClone();
        fire.stopTimes = new();
        return fire;
    }

    public override void OnFinished()
    {
    }

    public override bool CanApplyEffect(Entity target)
    {
        return target.gameObject.TryGetComponent<IDamageable>(out _);
    }

    protected override void PlayStatus()
    {
        OnFire?.Invoke();
    }
}
