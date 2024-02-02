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

    private IAttacker.HitDelegate onHit;
    private IAttacker.AttackDelegate onAttack;
    public IAttacker.HitDelegate OnHit { get => onHit; set => onHit = value; }
    public IAttacker.AttackDelegate OnAttack { get => onAttack; set => onAttack = value; }

    [SerializeField] private float range;
    [SerializeField] private float angle;

    protected override IEnumerator Brain()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            Entity[] entities = PhysicsExtensions.OverlapVisionCone(transform.position, angle, range, transform.forward)
                .Select(x => x.GetComponent<Entity>())
                .Where(x => x != null && x != this)
                .OrderBy(x => Vector3.Distance(x.transform.position, transform.position))
                .ToArray();

            Hero player = entities
                .Select(x => x.GetComponent<Hero>())
                .Where(x => x != null)
                .FirstOrDefault();

            if (player)
            {
                // Player detect
                MoveTo(player.transform.position);
            }
            else
            {
                // Random movement
                Vector2 rdmPos = Random.insideUnitCircle * range;
                MoveTo(transform.position + new Vector3(rdmPos.x, 0, rdmPos.y));
            }
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

    public void MoveTo(Vector3 posToMove)
    {
        agent.SetDestination(posToMove);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        //if (Selection.activeGameObject != gameObject)
        //    return;

        Entity[] entities = PhysicsExtensions.OverlapVisionCone(transform.position, angle, range, transform.forward)
            .Select(x => x.GetComponent<Entity>())
            .Where(x => x != null && x != this)
            .ToArray();

        Handles.color = new Color(1, 0, 0, 0.25f);
        if (entities.Length != 0)
        {
            Handles.color = new Color(0, 1, 0, 0.25f);
        }

        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, angle / 2f, range);
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -angle / 2f, range);

        Handles.color = Color.white;
        Handles.DrawWireDisc(transform.position, Vector3.up, range);

        // Debug text
        Handles.Label(
            transform.position + transform.up,
            "Range" +
            "\n - Health : " + stats.GetValueStat(Stat.HP) +
            "\n - Speed : " + stats.GetValueStat(Stat.SPEED),
            new GUIStyle()
            {
                alignment = TextAnchor.MiddleLeft,
                normal = new GUIStyleState()
                {
                    textColor = Color.white,
                }
            });
    }
#endif
}

// Quand dans zone de trigger, approche le joueur
// Quand en range, l'attaque
// Si le joueur s'est rapproché, commence à fuir
// Si le joueur est à une distance convenable, le ré-attaque
// Sinon, au bout de 2s, se retourne et l'attaque

// Plus tard, essayer de se cacher derrière un tank