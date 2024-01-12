using UnityEngine;

[RequireComponent(typeof(PlayerAnimation))]
public class Hero : Entity, IDamageable, IAttacker
{
    IAttacker.AttackDelegate onAttack;
    IAttacker.AttackDelegate onHit;
    public enum PlayerState : int
    {
        MOVE = EntityState.NB,
        DASH,
        ATTACK,
        HIT,
        DEAD
    }

    PlayerAnimation playerAnim;
    public IAttacker.AttackDelegate OnAttack { get => onAttack; set => onAttack = value; }
    public IAttacker.AttackDelegate OnHit { get => onHit; set => onHit = value; }

    private void Start()
    {
        playerAnim = GetComponent<PlayerAnimation>();
    }

    public void ApplyDamage(int _value)
    {
        Stats.IncreaseValue(Stat.HP, _value);
        if(_value < 0) //just to be sure it really inflicts damages
        {
            playerAnim.animator.SetTrigger("Hit");
        }
    }

    public void LaunchAttack()
    {
        OnAttack.Invoke();
    }
}