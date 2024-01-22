using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(VisionCone))]
public abstract class Mobs : Entity, IDamageable
{
    [SerializeField] Drop drops;
    protected NavMeshAgent agent;
    protected VisionCone visionCone;

    protected void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.speed = stats.GetValueStat(Stat.SPEED);

        OnDeath += drops.DropLoot;

        visionCone = GetComponent<VisionCone>();
    }

    protected Transform target = null;

    public enum EnemyState : int
    {
        WANDERING = EntityState.NB,
        TRIGGERED,
        DASH,
        FLEEING
    }

    public void ApplyDamage(int _value)
    {
        Stats.IncreaseValue(Stat.HP, -_value);
    }

    protected virtual void Update()
    {
        if (this.stats.GetValueStat(Stat.HP) < 0)
        {
            OnDeath?.Invoke(this.transform.position);
            Destroy(this.gameObject);
        }
    }

    public void HitPlayer()
    {
        int damage = (int)stats.GetValueStat(Stat.ATK) * (int)stats.GetValueStat(Stat.ATK_COEFF);
        Hero playerScript = target.gameObject.GetComponent<Hero>();

        playerScript.ApplyDamage(-damage);
    }

    private void OnDestroy()
    {
        OnDeath -= drops.DropLoot;
    }
}