using UnityEngine;
using UnityEngine.AI;

public class Electricity : Status
{
    private float entityBaseSpeed;

    public Electricity(float duration = 1f) : base(duration)
    {
        //entityBaseSpeed = target.Stats.GetValue(Stat.SPEED);
    }
    public override void ApplyEffect(Entity target)
    {
        if (target.Stats.HasStat(Stat.SPEED))
        {
            target.AddStatus(this);
            entityBaseSpeed = target.Stats.GetValue(Stat.SPEED);
            PlayVfx("VFX_Electricity");
        }
    }

    public override Status DeepCopy()
    {
        Electricity electricity = (Electricity)MemberwiseClone();
        return electricity;
    }

    public override void OnFinished()
    {
        target.Stats.SetValue(Stat.SPEED, entityBaseSpeed);
    }

    protected override void Effect()
    {
        if (target != null)
        {
            target.gameObject.GetComponent<NavMeshAgent>().speed = 0;
           //target.Stats.SetValue(Stat.SPEED, 0f);
        }
    }
}
