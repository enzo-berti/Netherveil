using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(VisionCone))]
public abstract class Mobs : Entity, IDamageable
{
    [SerializeField] Drop drops;
    protected NavMeshAgent agent;
    protected VisionCone visionCone;
    protected Transform target = null;

    private IDamageable.HitDelegate onHit;
    public IDamageable.HitDelegate OnHit { get => onHit; set => onHit = value; }

    public enum EnemyState : int
    {
        WANDERING = EntityState.NB,
        TRIGGERED,
        DASH,
        FLEEING
    }

    protected void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        visionCone = GetComponent<VisionCone>();

        agent.speed = stats.GetValueStat(Stat.SPEED);
    }

    private void OnEnable()
    {
        OnDeath += drops.DropLoot;
    }

    private void OnDisable()
    {
        OnDeath -= drops.DropLoot;
    }

    protected virtual void Update()
    {

    }

    public void ApplyDamage(int _value)
    {
        Stats.IncreaseValue(Stat.HP, -_value);

        if (stats.GetValueStat(Stat.HP) <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        OnDeath?.Invoke(transform.position);
        Destroy(gameObject);
    }
}