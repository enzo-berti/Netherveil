using UnityEngine;

[RequireComponent(typeof(PlayerAnimation))]
public class Hero : Entity, IDamageable
{
    PlayerAnimation playerAnim;

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
}