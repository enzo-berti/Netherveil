using PostProcessingEffects;
using UnityEngine;

public class Poison : OverTimeStatus
{
    private static readonly int baseStack = 3;
    static Color poisonColor = new Color(0.047f, 0.58f, 0.047f);
    public Poison(float _duration, float _chance) : base(_duration, _chance)
    {
        isStackable = true;
        frequency = duration/baseStack;
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

            if (Utilities.IsPlayer(target))
                PostProcessingEffectManager.current.Play(PostProcessingEffects.Effect.Poison);
        }
    }

    protected override void PlayStatus()
    {
        PlayVfx("VFX_Poison");
        AddStack(baseStack - 1);
    }
}
