using System.Linq;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

public class Fire : Status
{
    private int damage = 10;
    static Color fireColor = new Color(0.929f, 0.39f, 0.08f, 1.0f);
    VisualEffect vfx;

    public Fire(float _duration) : base(_duration)
    {
        statusChance = 1.0f;
        frequency = 0.5f;
    }

    public override void ApplyEffect(Entity target)
    {
        if (target.gameObject.TryGetComponent<IDamageable>(out _))
        {
            AddStack(1);
            target.AddStatus(this);
            vfx = GameObject.Instantiate(Resources.Load<GameObject>("VFX_Fire")).GetComponent<VisualEffect>();
            vfx.SetSkinnedMeshRenderer("New SkinnedMeshRenderer", target.gameObject.GetComponentInChildren<SkinnedMeshRenderer>());
            vfx.GetComponent<VFXPropertyBinder>().GetPropertyBinders<VFXTransformBinderCustom>().ToArray()[0].Target = target.gameObject.GetComponentInChildren<VFXTarget>().transform;
            vfx.Play();
        }
    }

    public override void OnFinished()
    {
        GameObject.Destroy(vfx.gameObject);
    }

    protected override void Effect()
    {
        if (target != null)
        {
            FloatingTextGenerator.CreateEffectDamageText(damage * Stack, target.transform.position, fireColor);
            target.gameObject.GetComponent<IDamageable>().ApplyDamage(damage * Stack, false, false);
        }
    }

    public override Status DeepCopy()
    {
        Fire fire = (Fire)this.MemberwiseClone();
        return fire;
    }
}
