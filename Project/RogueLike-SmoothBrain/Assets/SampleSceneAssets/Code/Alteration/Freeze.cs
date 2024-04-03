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

    private int damage = 10;
    static Color freezeColor = new Color(0.11f, 0.78f, 0.87f, 1.0f);

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
        if (target != null)
        {
            target.gameObject.GetComponent<NavMeshAgent>().speed = 0;
        }
    }

    public override void OnFinished()
    {
        target.gameObject.GetComponent<NavMeshAgent>().speed = baseAgentSpeed;
    }
}
