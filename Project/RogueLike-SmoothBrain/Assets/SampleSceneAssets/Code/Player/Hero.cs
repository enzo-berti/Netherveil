using UnityEngine;

[RequireComponent(typeof(PlayerAnimation))]
public class Hero : Entity, IDamageable
{
    public enum PlayerState : int
    {
        MOVE = EntityState.NB,
        DASH,
        ATTACK,
        HIT,
        DEAD
    }

    PlayerAnimation playerAnim;

    private void Start()
    {
        playerAnim = GetComponent<PlayerAnimation>();
    }

    public void ApplyDamage(int _value)
    {
        Stats.IncreaseValue(Stat.HP, _value);
        if(_value < 0 && stats.GetValueStat(Stat.HP) > 0) //just to be sure it really inflicts damages
        {
            State = (int)PlayerState.HIT;
            playerAnim.animator.ResetTrigger("Hit");
            playerAnim.animator.SetTrigger("Hit");
        }

        if(stats.GetValueStat(Stat.HP) <= 0 && State != (int) PlayerState.DEAD) 
        {
            State = (int)PlayerState.DEAD;
            playerAnim.animator.ResetTrigger("Death");
            playerAnim.animator.SetTrigger("Death");
        }
    }
}