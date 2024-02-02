using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Range : Mobs, IDamageable, IAttacker, IMovable
{
    private new enum State
    {
        MOVE,
        ATTACK,
        HIT,
        DEAD,
        TRIGGERED,
        FLEEING,
        WANDERING
    }

    Animator animator;

    private IAttacker.HitDelegate onHit;
    private IAttacker.AttackDelegate onAttack;
    public IAttacker.HitDelegate OnHit { get => onHit; set => onHit = value; }
    public IAttacker.AttackDelegate OnAttack { get => onAttack; set => onAttack = value; }

    [SerializeField] private float range;
    [SerializeField] private float angle;

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

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Collider[] collide = PhysicsExtensions.OverlapVisionCone(transform.position, angle, range, transform.forward)
            .Where(x => x.CompareTag("Player"))
            .ToArray();

        Handles.color = new Color(1, 0, 0, 0.25f);
        if (collide.Length != 0)
        {
            Handles.color = new Color(0, 1, 0, 0.25f);
        }

        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, angle / 2f, range);
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -angle / 2f, range);

        Handles.color = Color.white;
        Handles.DrawWireDisc(transform.position, Vector3.up, range);
    }
#endif
}

// Quand dans zone de trigger, approche le joueur
// Quand en range, l'attaque
// Si le joueur s'est rapproché, commence à fuir
// Si le joueur est à une distance convenable, le ré-attaque
// Sinon, au bout de 2s, se retourne et l'attaque

// Plus tard, essayer de se cacher derrière un tank