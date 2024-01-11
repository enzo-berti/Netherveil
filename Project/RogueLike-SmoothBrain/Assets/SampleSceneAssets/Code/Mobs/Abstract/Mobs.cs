using UnityEngine;

public abstract class Mobs : Entity, IDamageable
{

    protected EnemyState state;
    public EnemyState State {  get { return state; } }

    protected Transform target = null;

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
        Stats.IncreaseValue(Stat.HP, -_value);
    }

    void HitPlayer()
    {
        int damage = (int)stats.GetValueStat(Stat.ATK) * (int)stats.GetValueStat(Stat.ATK_COEFF);
        Hero playerScript = target.gameObject.GetComponent<Hero>();

        playerScript.ApplyDamage(-damage);
    }
}