using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

public class DamoclesSword : ConstantStatus
{
    VisualEffect vfx;
    int damages;

    public DamoclesSword(float _duration, float _chance) : base(_duration, _chance)
    {
        isStackable = false;
        damages = (int)(Utilities.Hero.Stats.GetValueWithoutCoeff(Stat.ATK)*2 * Utilities.Hero.Stats.GetCoeff(Stat.ATK));
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

        AudioManager.Instance.PlaySound(AudioManager.Instance.AresBladeSFX, target.transform.position);

        Physics.OverlapSphere(target.transform.position, vfx.GetAnimationCurve("SizeSlash").keys.Last().value / 2f, LayerMask.GetMask("Entity"))
        .Where(entity => entity.gameObject != (launcher as MonoBehaviour).gameObject)
        .Select(entity => entity.GetComponent<IDamageable>())
        .Where(entity => entity != null)
        .ToList()
        .ForEach(currentEntity =>
        {
            FloatingTextGenerator.CreateEffectDamageText(damages, (currentEntity as MonoBehaviour).transform.position, Hero.corruptionColor);
            currentEntity.ApplyDamage(damages, launcher, false);
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
