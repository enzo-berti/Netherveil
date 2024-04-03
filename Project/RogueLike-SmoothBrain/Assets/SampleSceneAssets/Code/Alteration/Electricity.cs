using UnityEngine;
using UnityEngine.VFX.Utility;
using UnityEngine.VFX;
using System.Linq;

public class Electricity : Status
{
    private float entityBaseSpeed;
    VisualEffect vfx;

    public Electricity(float duration = 1f) : base(duration)
    {
        this.isConst = true;
        //entityBaseSpeed = entity.Stats.GetValue(Stat.SPEED);
    }
    public override void ApplyEffect(Entity target)
    {
        if (target.Stats.HasStat(Stat.SPEED))
        {
            target.AddStatus(this);
            entityBaseSpeed = target.Stats.GetValue(Stat.SPEED);
            vfx = GameObject.Instantiate(Resources.Load<GameObject>("VFX_Fire")).GetComponent<VisualEffect>();
            vfx.SetSkinnedMeshRenderer("New SkinnedMeshRenderer", target.gameObject.GetComponentInChildren<SkinnedMeshRenderer>());
            vfx.GetComponent<VFXPropertyBinder>().GetPropertyBinders<VFXTransformBinderCustom>().ToArray()[0].Target = target.gameObject.GetComponentInChildren<VFXTarget>().transform;
            vfx.Play();
        }
    }

    public override Status DeepCopy()
    {
        throw new System.NotImplementedException();
    }

    public override void OnFinished()
    {
        target.Stats.SetValue(Stat.SPEED, entityBaseSpeed);
        GameObject.Destroy(vfx.gameObject);
    }

    protected override void Effect()
    {
        if (target != null)
        {
            target.Stats.SetValue(Stat.SPEED, 0f);
        }
    }
}
