using UnityEngine;

public class Fire : Status
{
    private int damage = 10;
    public Fire()
    {
        duration = 10.0f;
        this.frequency = 0.5f;
    }
    public Fire(Entity entity) : base(entity)
    {
        duration = 10.0f;
        this.frequency = 0.5f;
        damage *= (int)entity.Stats.GetValue(Stat.STATUS_POWER);
        duration *= (int)entity.Stats.GetValue(Stat.STATUS_DURATION);
    }
    public override void ApplyEffect(Entity target)
    {
        if (target.gameObject.TryGetComponent<IDamageable>(out _))
        {
            this.stack++;
            target.AddStatus(this);
        }
    }

    public override void OnFinished()
    {
    }

    protected override void Effect()
    {
        target.gameObject.GetComponent<IDamageable>().ApplyDamage(damage * stack);
    }
}
