public interface IAttacker 
{
    public delegate void AttackDelegate();
    public delegate void HitDelegate(Entity entity);
    public AttackDelegate OnAttack
    {
        get;
        set;
    }

    public HitDelegate OnHit
    {
        get;
        set;
    }
}
