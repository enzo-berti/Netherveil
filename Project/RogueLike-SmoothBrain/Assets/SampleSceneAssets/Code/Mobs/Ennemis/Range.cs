using UnityEngine;

public class Range : Sbire
{
    Animator animator;
    private bool isFleeing;
    private float fleeTimer;

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


        if (enemyToTargetVector.magnitude <= stats.GetValueStat(Stat.ATK_RANGE) / 2f && !isAttacking)
        {
            isFleeing = true;
            agent.SetDestination(transform.position - enemyToTargetVector);
        }

        if (isFleeing)
        {
            Flee(enemyToTargetVector);
        }
        else
        {
            if (State == (int)EnemyState.TRIGGERED)
            {
                agent.SetDestination(target.position);
            }
            else if (State == (int)EntityState.ATTACK)
            {
                AttackPlayer();
            }
        }
    }

    private void Flee(Vector3 _enemyToTargetVector)
    {
        fleeTimer += Time.deltaTime;

        if (fleeTimer > 2f || _enemyToTargetVector.magnitude >= stats.GetValueStat(Stat.ATK_RANGE) * 0.25f)
        {
            isFleeing = false;
        }
    }

}

// Quand dans zone de trigger, approche le joueur
// Quand en range, l'attaque
// Si le joueur s'est rapproché, commence à fuir
// Si le joueur est à une distance convenable, le ré-attaque
// Sinon, au bout de 2s, se retourne et l'attaque

// Plus tard, essayer de se cacher derrière un tank