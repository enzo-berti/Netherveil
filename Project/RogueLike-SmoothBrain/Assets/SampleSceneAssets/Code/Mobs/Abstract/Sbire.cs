using UnityEngine;

public class Sbire : Mobs
{
    protected float cooldown = 0;
    protected bool isAttacking = false;

    protected override void Update()
    {
        base.Update();

        Vector3 enemyToTargetVector = Vector3.zero;

        if (target != null)
        {
            enemyToTargetVector = target.position - transform.position;
            enemyToTargetVector.y = 0;

            if (enemyToTargetVector.magnitude <= stats.GetValueStat(Stat.ATK_RANGE))
                State = (int)EntityState.ATTACK;
            else
                State = (int)EnemyState.TRIGGERED;
        }

        if (State != (int)EntityState.ATTACK)
        {
            cooldown = 0;
        }
        else
        {
            agent.SetDestination(transform.position);
        }

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
                FollowPlayer();
                break;

            case (int)EnemyState.DASH:
                break;

            default:
                break;
        }
    }

    protected void FollowPlayer()
    {
        agent.SetDestination(target.position);
    }

    // fait sa vie, se balade dans la salle
    protected void Wander()
    {

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            State = (int)EnemyState.TRIGGERED;
            target = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            State = (int)EnemyState.WANDERING;
            target = null;
        }
    }
}