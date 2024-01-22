using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(VisionCone))]
public abstract class Mobs : Entity
{
    [SerializeField] Drop drops;
    protected NavMeshAgent agent;
    protected VisionCone visionCone;
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
}