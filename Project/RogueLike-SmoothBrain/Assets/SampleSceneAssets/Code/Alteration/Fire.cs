using System;
using System.Collections;
using UnityEngine;

public class Fire : OverTimeStatus
{
    private int damage = 10;
    static Color fireColor = new Color(0.929f, 0.39f, 0.08f, 1.0f);
    public static event Action OnFire;
    public bool firstEffect = true;
    public Fire(float _duration, float _statusChance) : base(_duration, _statusChance)
    {
        frequency = 0.5f;
        isStackable = true;
    }
    protected override void Effect()
    {
        if(firstEffect)
        {
            firstEffect = false;
            OnFire?.Invoke();
        }
        if (target != null)
        {
            FloatingTextGenerator.CreateEffectDamageText(damage * Stack, target.transform.position, fireColor);
            target.gameObject.GetComponent<IDamageable>().ApplyDamage(damage * Stack, launcher, false);
        }
    }

    public override Status DeepCopy()
    {
        Fire fire = (Fire)this.MemberwiseClone();
        return fire;
    }

    public override void OnFinished()
    {
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
