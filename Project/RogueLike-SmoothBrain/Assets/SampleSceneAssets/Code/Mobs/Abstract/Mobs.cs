using System.Collections;
using UnityEngine.AI;

public abstract class Mobs : Entity
{
    protected NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = stats.GetValueStat(Stat.SPEED);
    }

    private void Start()
    {
        StartCoroutine(Brain());
    }

    protected abstract IEnumerator Brain();
}