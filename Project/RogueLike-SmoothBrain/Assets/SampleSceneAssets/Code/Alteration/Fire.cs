using UnityEngine;

public class Fire : Status
{
    private int damage = 10;

    public Fire(float _duration) : base(_duration)
    {
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

    public override Status ShallowCopy()
    {
        return (Fire)this.MemberwiseClone();
    }

    protected override void Effect()
    {
        target.gameObject.GetComponent<IDamageable>().ApplyDamage(damage * stack);
    }
}
