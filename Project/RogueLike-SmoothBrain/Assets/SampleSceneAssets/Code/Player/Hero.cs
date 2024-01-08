public class Hero : Entity, IDamageable
{
    public void ApplyDamage(int _value)
    {
        Stats.IncreaseValue(Stat.HP, _value);
    }
}