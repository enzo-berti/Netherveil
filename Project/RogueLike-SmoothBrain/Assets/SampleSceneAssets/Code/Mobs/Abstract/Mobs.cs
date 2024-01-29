using UnityEngine.AI;

public abstract class Mobs : Entity
{
    protected NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = stats.GetValueStat(Stat.SPEED);
    }
}