using UnityEngine.AI;

public abstract class Mobs : Entity
{
    protected NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }
}