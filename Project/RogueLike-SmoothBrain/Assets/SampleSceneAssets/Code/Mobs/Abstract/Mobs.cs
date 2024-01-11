using Unity.VisualScripting;
using UnityEngine;

public abstract class Mobs : Entity, IDamageable
{

    protected EnemyState state;
    public EnemyState State {  get { return state; } }

    [SerializeField] Drop drops;
    private void Start()
    {
        OnDeath += drops.DropLoot;
    }
	
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

    public void Update()
    {
        if (this.stats.GetValueStat(Stat.HP) < 0)
        {
            OnDeath?.Invoke(this.transform.position);
            Destroy(this.gameObject);
        }
    }
    void HitPlayer()
    {
        int damage = (int)stats.GetValueStat(Stat.ATK) * (int)stats.GetValueStat(Stat.ATK_COEFF);
        Hero playerScript = target.gameObject.GetComponent<Hero>();

        playerScript.ApplyDamage(-damage);
    }
}