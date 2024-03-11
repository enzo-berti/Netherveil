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

    public override void OnFinished()
    {
       target.Stats.SetValue(Stat.SPEED, entityBaseSpeed);
    }

    public override Status ShallowCopy()
    {
        return (Electricity)this.MemberwiseClone();
    }

    protected override void Effect()
    {
        target.Stats.SetValue(Stat.SPEED, 0f);
    }
}
