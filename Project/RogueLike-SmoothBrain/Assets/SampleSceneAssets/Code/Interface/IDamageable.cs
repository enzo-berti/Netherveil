public interface IDamageable
{
    void ApplyDamage(int _value, bool hasAnimation = true);
    void Death();
}
