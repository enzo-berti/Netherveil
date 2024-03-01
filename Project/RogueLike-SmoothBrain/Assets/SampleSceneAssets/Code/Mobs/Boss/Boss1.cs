using System.Collections;
using System.Linq;
using UnityEngine;

public class Boss1 : Mobs, IAttacker, IDamageable, IMovable, IBlastable
{
    private IAttacker.AttackDelegate onAttack;
    private IAttacker.HitDelegate onHit;
    public IAttacker.AttackDelegate OnAttack { get => onAttack; set => onAttack = value; }
    public IAttacker.HitDelegate OnHit { get => onHit; set => onHit = value; }

    [Header("Boss Parameters")]
    [SerializeField, Range(0f, 360f)] private float angle = 360f;

    enum Attacks
    {
        RANGE,
        THRUST,
        DASH,
        NONE
    }

    enum AttackState
    {
        CHARGING,
        ATTACKING,
        RECOVERING,
        IDLE
    }

    Attacks currentAttack;
    AttackState attackState;
    float attackCooldown = 0;
    bool hasProjectile = false;

    protected override IEnumerator Brain()
    {
        while (true)
        {
            yield return null;

            Entity[] entities = PhysicsExtensions.OverlapVisionCone(transform.position, angle, (int)stats.GetValueStat(Stat.VISION_RANGE), transform.forward, LayerMask.GetMask("Entity"))
                .Select(x => x.GetComponent<Entity>())
                .Where(x => x != null && x != this)
                .OrderBy(x => Vector3.Distance(x.transform.position, transform.position))
                .ToArray();

            Hero player = entities
                .Select(x => x.GetComponent<Hero>())
                .Where(x => x != null)
                .FirstOrDefault();

            if (attackCooldown > 0)
            {
                attackState = AttackState.IDLE;
                attackCooldown -= Time.deltaTime;
                if (attackCooldown < 0) attackCooldown = 0;
            }
            else if (attackCooldown == 0)
            {
                currentAttack = (Attacks)Random.Range(0, 3);
            }



        }
    }
    protected override IEnumerator EntityDetection()
    {
        while (true)
        {
            yield return null;
        }
    }

    public void Attack(IDamageable _damageable)
    {
        _damageable.ApplyDamage((int)(stats.GetValueStat(Stat.ATK) * stats.GetValueStat(Stat.ATK_COEFF)));
    }

    public void ApplyDamage(int _value)
    {
        Stats.IncreaseValue(Stat.HP, -_value, false);
        if (stats.GetValueStat(Stat.HP) <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        Destroy(gameObject);
        GameObject.FindWithTag("Player").GetComponent<Hero>().OnKill?.Invoke(this);
    }

    public void MoveTo(Vector3 _pos)
    {
        agent.SetDestination(_pos);
    }

}
