using UnityEngine;
using UnityEngine.AI;

public abstract class Mobs : Entity
{
    [SerializeField] Drop drops;
    protected NavMeshAgent agent;

    public enum EnemyState : int
    {
        WANDERING = EntityState.NB,
        TRIGGERED,
        DASH,
        FLEEING,
        SEARCHING
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