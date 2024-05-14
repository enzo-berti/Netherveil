using PostProcessingEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;
using static UnityEngine.EventSystems.EventTrigger;

public class DamoclesSword : ConstantStatus
{
    VisualEffect vfx;

    public DamoclesSword(float _duration, float _chance) : base(_duration, _chance)
    {
        isStackable = false;
    }

    public override Status DeepCopy()
    {
        DamoclesSword damoclesSword = (DamoclesSword)MemberwiseClone();
        return damoclesSword;
    }

    protected override void Effect()
    {
    }

    public override void OnFinished()
    {
        if (target == null)
            return;

        Physics.OverlapSphere(target.transform.position, vfx.GetAnimationCurve("SizeSlash").keys.Last().value / 2f, LayerMask.GetMask("Entity"))
        .Where(entity => entity.gameObject != (launcher as MonoBehaviour).gameObject)
        .Select(entity => entity.GetComponent<IDamageable>())
        .Where(entity => entity != null)
        .ToList()
        .ForEach(currentEntity =>
        {
            FloatingTextGenerator.CreateEffectDamageText(10, (currentEntity as MonoBehaviour).transform.position, Hero.corruptionColor);
            currentEntity.ApplyDamage(10, launcher, false);
        });

        GameObject.Destroy(vfx.gameObject, 0.3f);
    }

    public override bool CanApplyEffect(Entity target)
    {
        return true;
    }

    protected override void PlayStatus()
    {
        vfx = GameObject.Instantiate(GameResources.Get<GameObject>("VFX_CorruptedSword"), target.transform).GetComponent<VisualEffect>();
        vfx.SetFloat("Duration", duration + 0.2f);
        vfx.Play();
    }
}
