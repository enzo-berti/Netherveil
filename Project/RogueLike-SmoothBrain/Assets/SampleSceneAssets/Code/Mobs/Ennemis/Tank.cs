using UnityEngine;
using UnityEngine.AI;

public class Tank : Sbire, IDamageable, IAttacker, IMovable
{
    Animator animator;

    private IAttacker.HitDelegate onHit;
    private IAttacker.AttackDelegate onAttack;
    public IAttacker.HitDelegate OnHit { get => onHit; set => onHit = value; }
    public IAttacker.AttackDelegate OnAttack { get => onAttack; set => onAttack = value; }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = stats.GetValueStat(Stat.SPEED);

        animator = GetComponent<Animator>();
    }

    protected override void Update()
    {
        base.Update();

        animator.SetBool("InAttackRange", State == (int)EntityState.ATTACK);
        animator.SetBool("Triggered", State == (int)EnemyState.TRIGGERED || State == (int)EntityState.ATTACK || agent.hasPath);
        animator.SetBool("Punch", isAttacking);
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

    public void MoveTo(Vector3 posToMove)
    {
        agent.SetDestination(posToMove);
    }
}
