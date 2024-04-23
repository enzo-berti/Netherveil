using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : OverTimeStatus
{
    private static readonly int baseStack = 15;
    static Color poisonColor = new Color(0.047f, 0.58f, 0.047f);
    public Poison(float _duration, float _chance) : base(_duration, _chance)
    {
        isStackable = true;
        frequency = 0.5f;
        duration = frequency;
    }

    public override bool CanApplyEffect(Entity target)
    {
        return target.TryGetComponent<IDamageable>(out _);
    }

    public override Status DeepCopy()
    {
        Poison poison = (Poison)this.MemberwiseClone();
        poison.stopTimes = new();
        return poison;
    }

    public override void OnFinished()
    {
    }

    protected override void Effect()
    {
        if(target != null)
        {
            FloatingTextGenerator.CreateEffectDamageText(Stack, target.transform.position, poisonColor);
            target.gameObject.GetComponent<IDamageable>().ApplyDamage(Stack, launcher, false);
        }
    }

    protected override void PlayStatus()
    {
        AddStack(baseStack - 1);
    }
}
