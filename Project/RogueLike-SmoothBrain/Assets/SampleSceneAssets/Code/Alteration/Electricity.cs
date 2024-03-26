using static UnityEngine.EventSystems.EventTrigger;

public class Electricity : Status
{
    private float entityBaseSpeed;
    public Electricity(float duration = 1f) : base(duration)
    {
        this.isConst = true;
        //entityBaseSpeed = entity.Stats.GetValue(Stat.SPEED);
    }
    public override void ApplyEffect(Entity target)
    {
        if (target.Stats.HasStat(Stat.SPEED))
            target.AddStatus(this);
            entityBaseSpeed = target.Stats.GetValue(Stat.SPEED);
    }

    public override Status DeepCopy()
    {
        throw new System.NotImplementedException();
    }

    public override void OnFinished()
    {
       target.Stats.SetValue(Stat.SPEED, entityBaseSpeed);
    }

    protected override void Effect()
    {
        if (target != null)
        {
            target.Stats.SetValue(Stat.SPEED, 0f);
        }
    }
}
