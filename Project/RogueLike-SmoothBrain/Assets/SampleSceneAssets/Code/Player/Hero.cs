using UnityEngine;

public class Hero : Entity, IDamageable, IAttacker
{
    public enum PlayerState : int
    {
        DASH = EntityState.NB
    }

    Animator animator;
    Inventory inventory = new Inventory();
    public Inventory Inventory { get { return inventory; } }

    private IAttacker.AttackDelegate onAttack;
    private IAttacker.HitDelegate onHit;

    public IAttacker.AttackDelegate OnAttack { get => onAttack; set => onAttack = value; }
    public IAttacker.HitDelegate OnHit { get => onHit; set => onHit = value; }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ApplyDamage(int _value)
    {
        Stats.IncreaseValue(Stat.HP, -_value);
        if ((-_value) < 0 && stats.GetValueStat(Stat.HP) > 0) //just to be sure it really inflicts damages
        {
            State = (int)EntityState.HIT;
            animator.ResetTrigger("Hit");
            animator.SetTrigger("Hit");
        }

        if (stats.GetValueStat(Stat.HP) <= 0 && State != (int)EntityState.DEAD)
        {
            Death();
        }
    }

    public void Death()
    {
        State = (int)EntityState.DEAD;
        animator.ResetTrigger("Death");
        animator.SetTrigger("Death");
    }

    public void Attack(IDamageable damageable)
    {
        damageable.ApplyDamage((int)(stats.GetValueStat(Stat.ATK) * stats.GetValueStat(Stat.ATK_COEFF)));
        onAttack?.Invoke(damageable);
    }

    public void LifeSteal(IDamageable damageable)
    {
        int lifeIncreasedValue = (int)(Stats.GetValueStat(Stat.LIFE_STEAL) * (Stats.GetValueStat(Stat.ATK) * Stats.GetValueStat(Stat.ATK_COEFF)));
        Stats.IncreaseValue(Stat.HP, lifeIncreasedValue);
        if (Stats.GetValueStat(Stat.HP) > Stats.GetValueStat(Stat.MAX_HP))
        {
            Stats.SetValue(Stat.HP, Stats.GetValueStat(Stat.MAX_HP));
        }
    }

    private void OnEnable()
    {
        onHit += LifeSteal;
    }

    private void OnDisable()
    {
        onHit -= LifeSteal;
    }
}