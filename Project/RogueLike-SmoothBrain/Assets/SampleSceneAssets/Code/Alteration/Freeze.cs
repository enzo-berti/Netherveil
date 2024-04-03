using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class Freeze : Status
{
    float baseAgentSpeed;
    public Freeze(float duration) : base(duration)
    {
        isStackable = false;
    }

    public override void ApplyEffect(Entity target)
    {
        if (target.Stats.HasStat(Stat.SPEED))
        {
            baseAgentSpeed = target.gameObject.GetComponent<NavMeshAgent>().speed;
            target.AddStatus(this);
            PlayVfx("VFX_Frozen");
        }
    }

    public override Status DeepCopy()
    {
        Freeze freeze = (Freeze)MemberwiseClone();
        return freeze;
    }

    protected override void Effect()
    {
        if(target != null)
        { 
            target.gameObject.GetComponent<NavMeshAgent>().speed = 0;
        }
    }

    public override void OnFinished()
    {
        target.gameObject.GetComponent<NavMeshAgent>().speed = baseAgentSpeed;
    }
}
