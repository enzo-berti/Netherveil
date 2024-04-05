using System.Collections.Generic;

public interface IAttacker 
{
    public List<Status> StatusToApply { get; }
    public delegate void AttackDelegate();
    public AttackDelegate OnAttack
    {
        get;
        set;
    }

    public delegate void HitDelegate(IDamageable damageable, IAttacker attacker);
    public HitDelegate OnHit
    {
        get;
        set;
    }

    public void Attack(IDamageable damageable);

    public void ApplyStatus(IDamageable damageable, IAttacker attacker)
    {
        Entity entity = damageable as Entity;
        if (entity == null) return;
        foreach (var status in StatusToApply)
        {
            entity.ApplyEffect(status.DeepCopy());
        }
    }
}
