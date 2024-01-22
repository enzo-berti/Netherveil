using UnityEngine;

[RequireComponent(typeof(PlayerAnimation))]
public class Hero : Entity, IDamageable, IAttacker
{
    public enum PlayerState : int
    {
        DASH = EntityState.NB
    }

    PlayerAnimation playerAnim;
    Inventory inventory = new Inventory();
    public Inventory Inventory { get { return inventory; } }

    private IAttacker.AttackDelegate onAttack;
    private IDamageable.HitDelegate onHit;

    public IAttacker.AttackDelegate OnAttack { get => onAttack; set => onAttack = value; }
    public IDamageable.HitDelegate OnHit { get => onHit; set => onHit = value; }

    private void Start()
    {
        playerAnim = GetComponent<PlayerAnimation>();
    }

    public void ApplyDamage(int _value)
    {
        Stats.IncreaseValue(Stat.HP, _value);
        if (_value < 0 && stats.GetValueStat(Stat.HP) > 0) //just to be sure it really inflicts damages
        {
            State = (int)EntityState.HIT;
            playerAnim.animator.ResetTrigger("Hit");
            playerAnim.animator.SetTrigger("Hit");
        }

        if (stats.GetValueStat(Stat.HP) <= 0 && State != (int)EntityState.DEAD)
        {
            Death();
        }
    }

    public void Death()
    {
        State = (int)EntityState.DEAD;
        playerAnim.animator.ResetTrigger("Death");
        playerAnim.animator.SetTrigger("Death");
    }

    public void Attack(IDamageable damageable)
    {
        damageable.ApplyDamage((int)(stats.GetValueStat(Stat.ATK) * stats.GetValueStat(Stat.ATK_COEFF)));
        onAttack?.Invoke(damageable);
    }
}