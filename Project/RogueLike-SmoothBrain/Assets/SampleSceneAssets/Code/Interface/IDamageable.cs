public interface IDamageable
{
    public delegate void HitDelegate(IAttacker attacker);
    public HitDelegate OnHit
    {
        get;
        set;
    }

    void ApplyDamage(int _value);
    void Death();
}
