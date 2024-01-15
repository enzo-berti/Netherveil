using UnityEngine;

public class Range : Sbire
{
    Animator animator;

    new private void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
    }

    protected override void Update()
    {
        base.Update();

        animator.SetBool("InAttackRange", State == (int)EntityState.ATTACK);
        animator.SetBool("Triggered", State == (int)EnemyState.TRIGGERED || State == (int)EntityState.ATTACK || agent.hasPath);
        animator.SetBool("Punch", isAttacking);
    }

    protected override void SimpleAI()
    {
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

        if (State == (int)EnemyState.TRIGGERED)
        {
            agent.SetDestination(transform.position - enemyToTargetVector);
        }
        else if (State == (int)EntityState.ATTACK)
        {
            AttackPlayer();
        }


    }
}
