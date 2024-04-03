using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class Freeze : Status
{
    float baseAgentSpeed;
    public Freeze(float duration) : base(duration)
    {
        isConst = true;
    }

    public override void ApplyEffect(Entity target)
    {
        if (target.Stats.HasStat(Stat.SPEED))
        {
            if(target.gameObject.GetComponent<NavMeshAgent>().speed != 0)
                baseAgentSpeed = target.gameObject.GetComponent<NavMeshAgent>().speed;
            target.AddStatus(this);
            target.gameObject.GetComponent<NavMeshAgent>().speed = 0;
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
    }

    public override void OnFinished()
    {
        target.gameObject.GetComponent<NavMeshAgent>().speed = baseAgentSpeed;
    }
}
