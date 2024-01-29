using UnityEngine;
using UnityEngine.AI;

public abstract class Mobs : Entity
{
    [SerializeField] Drop drops;
    protected NavMeshAgent agent;
    protected Transform target = null;

    public enum EnemyState : int
    {
        WANDERING = EntityState.NB,
        TRIGGERED,
        DASH,
        FLEEING,
        SEARCHING
    }

    protected void Start()
    {
        agent = GetComponent<NavMeshAgent>();

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
}