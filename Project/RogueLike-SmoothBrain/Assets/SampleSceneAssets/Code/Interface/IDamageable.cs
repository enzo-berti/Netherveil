public interface IDamageable
{
    void ApplyDamage(int _value, bool isCrit = false, bool hasAnimation = true);
    void Death();
}
