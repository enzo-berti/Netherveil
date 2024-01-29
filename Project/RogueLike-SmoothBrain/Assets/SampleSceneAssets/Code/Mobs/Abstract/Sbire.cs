using UnityEngine;

public class Sbire : Mobs
{
    protected float cooldown = 0;
    protected bool isAttacking = false;
    Vector3 lastKnownTarget = Vector3.zero;
    [SerializeField] float rotationSpeed = 5f;
    private Transform target;

    protected override void Update()
    {
        base.Update();

        lastKnownTarget = transform.position;

        SimpleAI();
    }

    protected virtual void SimpleAI()
    {
        // Si le joueur est dans le cone de vision
        if (target)
        {
            Vector3 enemyToTargetVector = Vector3.zero;

            enemyToTargetVector = target.position - transform.position;
            enemyToTargetVector.y = 0;

            if (enemyToTargetVector.magnitude <= stats.GetValueStat(Stat.ATK_RANGE))
                State = (int)EntityState.ATTACK;
            else
                State = (int)EnemyState.TRIGGERED;
        }

        // Reset le cooldown de l'attaque et fige l'ennemi en place
        if (State != (int)EntityState.ATTACK)
        {
            cooldown = 0;
        }
        else
        {
            agent.SetDestination(transform.position);
        }

        if (State == (int)EntityState.ATTACK || State == (int)EnemyState.TRIGGERED) RotateTowardsTarget();

        // StateMachine
        switch (State)
        {
            case (int)EntityState.ATTACK:
                AttackPlayer();
                break;

            case (int)EntityState.HIT:
                break;

            case (int)EntityState.DEAD:
                break;

            case (int)EnemyState.WANDERING:
                break;

            case (int)EnemyState.TRIGGERED:
                break;

            case (int)EnemyState.DASH:
                break;

            case (int)EnemyState.SEARCHING:
                SearchPlayer();
                break;

            default:
                break;
        }
    }

    protected void AttackPlayer()
    {
        cooldown += Time.deltaTime;

        // tape FIRE_RATE fois par seconde
        if (cooldown >= 1f / stats.GetValueStat(Stat.FIRE_RATE))
        {
            isAttacking = true;
            cooldown = 0;
        }
        else
        {
            isAttacking = false;
        }
    }

    protected void SearchPlayer()
    {
        agent.SetDestination(lastKnownTarget);

        if (!agent.hasPath)
        {
            State = (int)EnemyState.WANDERING;
        }
    }

    protected void RotateTowardsTarget()
    {
        if (target)
        {
            Vector3 targetDirection = target.position - transform.position;
            targetDirection.y = 0f;

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}