using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grafted : Mobs, IAttacker, IDamageable, IMovable, IBlastable
{
    private IAttacker.AttackDelegate onAttack;
    private IAttacker.HitDelegate onHit;
    public IAttacker.AttackDelegate OnAttack { get => onAttack; set => onAttack = value; }
    public IAttacker.HitDelegate OnHit { get => onHit; set => onHit = value; }

    [SerializeField, Range(0f, 360f)] private float visionAngle = 360f;

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

            Entity[] entities = PhysicsExtensions.OverlapVisionCone(transform.position, visionAngle, (int)stats.GetValue(Stat.VISION_RANGE), transform.forward, LayerMask.GetMask("Entity"))
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

            switch (currentAttack)
            {
                case Attacks.RANGE:
                    if (hasProjectile) ThrowProjectile(); else RetrieveProjectile();
                    break;

                case Attacks.THRUST:
                    TripleThrurst();
                    break;

                case Attacks.DASH:
                    Dash();
                    break;

                case Attacks.NONE:
                    break;
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
        _damageable.ApplyDamage((int)(stats.GetValue(Stat.ATK) * stats.GetValue(Stat.ATK_COEFF)));
    }

    public void ApplyDamage(int _value)
    {
        Stats.IncreaseValue(Stat.HP, -_value, false);
        DamageManager.Instance.CreateDamageText(_value, transform.position + Vector3.up * 2, false, 1);
        if (stats.GetValue(Stat.HP) <= 0)
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

    public void AttackCollide(List<Collider> colliders, bool debugMode = true)
    {
        if (debugMode)
        {
            foreach (Collider collider in colliders)
            {
                collider.gameObject.SetActive(true);
            }
        }

        Vector3 rayOffset = Vector3.up / 2;

        foreach (Collider spearCollider in colliders)
        {
            Collider[] tab = PhysicsExtensions.CheckAttackCollideRayCheck(spearCollider, transform.position + rayOffset, "Player", LayerMask.GetMask("Map"));
            if (tab.Length > 0)
            {
                foreach (Collider col in tab)
                {
                    if (col.gameObject.GetComponent<Hero>() != null)
                    {
                        //Debug.Log(col.gameObject.name);
                        Attack(col.gameObject.GetComponent<IDamageable>());
                        break;
                    }
                }
            }
        }
    }

    ///////////////////////////////// ATTACKS

    void ThrowProjectile()
    {
        hasProjectile = false;
        currentAttack = Attacks.NONE;
        attackState = AttackState.IDLE;
    }

    void RetrieveProjectile()
    {
        hasProjectile = true;
        currentAttack = Attacks.NONE;
        attackState = AttackState.IDLE;
    }

    void TripleThrurst()
    {
        currentAttack = Attacks.NONE;
        attackState = AttackState.IDLE;
    }

    void Dash()
    {
        currentAttack = Attacks.NONE;
        attackState = AttackState.IDLE;
    }
}