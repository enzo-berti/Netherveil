using UnityEngine;

[RequireComponent(typeof(PlayerAnimation))]
public class Hero : Entity, IDamageable, IAttacker
{
    IAttacker.AttackDelegate onAttack;
    IAttacker.AttackDelegate onHit;
    public enum PlayerState : int
    {
        DASH = EntityState.NB
    }

    PlayerAnimation playerAnim;
    Inventory inventory;
    public Inventory Inventory { get { return inventory; } }
    public IAttacker.AttackDelegate OnAttack { get => onAttack; set => onAttack = value; }
    public IAttacker.AttackDelegate OnHit { get => onHit; set => onHit = value; }

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
            State = (int)EntityState.DEAD;
            playerAnim.animator.ResetTrigger("Death");
            playerAnim.animator.SetTrigger("Death");
        }
    }

    public void LaunchAttack()
    {
        OnAttack?.Invoke();
    }
}