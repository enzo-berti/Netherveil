using UnityEngine;

public class Range : Sbire, IDamageable, IAttacker
{
    Animator animator;

    private IAttacker.HitDelegate onHit;
    private IAttacker.AttackDelegate onAttack;
    public IAttacker.HitDelegate OnHit { get => onHit; set => onHit = value; }
    public IAttacker.AttackDelegate OnAttack { get => onAttack; set => onAttack = value; }

    private float fleeTimer;
    private float fleeCooldown;

    new private void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
    }

    protected override void Update()
    {
        base.Update();

        animator.SetBool("InAttackRange", State == (int)EntityState.ATTACK);
        animator.SetBool("Triggered", State == (int)EnemyState.TRIGGERED || State == (int)EntityState.ATTACK || State == (int)EnemyState.FLEEING || agent.hasPath);
        animator.SetBool("Punch", isAttacking);
    }

    protected override void SimpleAI()
    {
        Vector3 enemyToTargetVector = Vector3.zero;

        // update le cooldown de la fuite
        fleeCooldown = (fleeCooldown > 0) ? fleeCooldown - Time.deltaTime : 0;

        // comportement de base, court et attaque le joueur
        if (target != null)
        {
            enemyToTargetVector = target.position - transform.position;
            enemyToTargetVector.y = 0;

            if (State != (int)EnemyState.FLEEING)
            {
                if (enemyToTargetVector.magnitude <= stats.GetValueStat(Stat.ATK_RANGE))
                    State = (int)EntityState.ATTACK;
                else
                    State = (int)EnemyState.TRIGGERED;
            }
        }

        // reset le cd d'attaque et la destination
        if (State != (int)EntityState.ATTACK)
        {
            cooldown = 0;
        }
        else
        {
            agent.SetDestination(transform.position);
        }

        // si le joueur est à une certaine distance du range, se met à fuir
        if (enemyToTargetVector.magnitude <= stats.GetValueStat(Stat.ATK_RANGE) * 0.5f && !isAttacking)
        {
            State = (int)EnemyState.FLEEING;
        }

        // fuis ou continue son comportement de base
        if (State == (int)EnemyState.FLEEING)
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
        agent.SetDestination(transform.position - _enemyToTargetVector);

        if (fleeTimer > 2f || _enemyToTargetVector.magnitude >= stats.GetValueStat(Stat.ATK_RANGE))
        {
            State = (int)EnemyState.WANDERING;
            fleeTimer = 0;
        }
    }

    public void ApplyDamage(int _value)
    {
        Stats.IncreaseValue(Stat.HP, -_value);

        if (stats.GetValueStat(Stat.HP) <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        OnDeath?.Invoke(transform.position);
        Destroy(gameObject);
    }

    public void Attack(IDamageable damageable)
    {
        OnAttack?.Invoke(damageable);
        damageable.ApplyDamage((int)(stats.GetValueStat(Stat.ATK) * stats.GetValueStat(Stat.ATK_COEFF)));
    }
}

// Quand dans zone de trigger, approche le joueur
// Quand en range, l'attaque
// Si le joueur s'est rapproché, commence à fuir
// Si le joueur est à une distance convenable, le ré-attaque
// Sinon, au bout de 2s, se retourne et l'attaque

// Plus tard, essayer de se cacher derrière un tank