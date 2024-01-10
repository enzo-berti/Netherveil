public abstract class Mobs : Entity, IDamageable
{

    protected EnemyState state;
    public EnemyState State {  get { return state; } }
    public enum EnemyState
    {
        WANDERING,
        TRIGGERED,
        MOVE,
        DASH,
        ATTACK,
        HIT,
        DEAD
    }
    public void ApplyDamage(int _value)
    {
        Stats.IncreaseValue(Stat.HP, _value);
    }
}