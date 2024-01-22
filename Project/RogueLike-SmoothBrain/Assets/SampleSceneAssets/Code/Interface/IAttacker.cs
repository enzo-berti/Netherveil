public interface IAttacker 
{
    public delegate void AttackDelegate(IDamageable damageable);
    public AttackDelegate OnAttack
    {
        get;
        set;
    }

    public delegate void HitDelegate(IDamageable damageable);
    public HitDelegate OnHit
    {
        get;
        set;
    }

    public void Attack(IDamageable damageable);
}
