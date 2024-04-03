using UnityEngine;
using UnityEngine.VFX.Utility;
using UnityEngine.VFX;
using System.Linq;

public class Freeze : Status
{
    VisualEffect vfx;
    public Freeze(float duration) : base(duration)
    {
        isConst = true;
    }

    private int damage = 10;
    static Color freezeColor = new Color(0.11f, 0.78f, 0.87f, 1.0f);

    public override void ApplyEffect(Entity target)
    {
        if (target.Stats.HasStat(Stat.SPEED))
        {
            target.AddStatus(this);
            vfx = GameObject.Instantiate(Resources.Load<GameObject>("VFX_Fire")).GetComponent<VisualEffect>();
            vfx.SetSkinnedMeshRenderer("New SkinnedMeshRenderer", target.gameObject.GetComponentInChildren<SkinnedMeshRenderer>());
            vfx.GetComponent<VFXPropertyBinder>().GetPropertyBinders<VFXTransformBinderCustom>().ToArray()[0].Target = target.gameObject.GetComponentInChildren<VFXTarget>().transform;
            vfx.Play();
        }
    }

    public override Status DeepCopy()
    {
        Freeze fire = (Freeze)MemberwiseClone();
        return fire;
    }

    public override void OnFinished()
    {
        GameObject.Destroy(vfx.gameObject);
    }


    protected override void Effect()
    {
        if (target != null)
        {
            FloatingTextGenerator.CreateEffectDamageText(damage * Stack, target.transform.position, freezeColor);
            target.gameObject.GetComponent<IDamageable>().ApplyDamage(damage * Stack, false, false);
        }
    }
}
