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

    [Header("Range Parameters")]
    [SerializeField, Range(0f, 360f)] private float angle = 120f;

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

            Tank[] tanks = PhysicsExtensions.OverlapVisionCone(transform.position, 360, (int)stats.GetValueStat(Stat.VISION_RANGE), transform.forward, LayerMask.GetMask("Entity"))
                .Select(x => x.GetComponent<Tank>())
                .Where(x => x != null)
                .OrderBy(x => Vector3.Distance(x.transform.position, transform.position))
                .ToArray();

            // Si le joueur est détecté 
            if (player)
            {
                // Si un tank est à proximité
                if (tanks.Any())
                {
                    {
                        Vector3 playerTankVector = tanks.First().transform.position - player.transform.position;

                        playerTankVector.Normalize();
                        Vector3 targetPos = player.transform.position + playerTankVector * stats.GetValueStat(Stat.ATK_RANGE);

                        MoveTo(targetPos);
                    }
                }
                else
                {
                    MoveTo(player.transform.position);
                }
            }

            //if (player)
            //{
            //    // Player detect
            //    MoveTo(player.transform.position);
            //}
            //else if (tanks.Any())
            //{
            //    // Other pest detect
            //    MoveTo(tanks.First().transform.position);
            //}
            //else
            //{
            //    // Random movement
            //    Vector2 rdmPos = Random.insideUnitCircle * (int)stats.GetValueStat(Stat.VISION_RANGE);
            //    //MoveTo(transform.position + new Vector3(rdmPos.x, 0, rdmPos.y));
            //}
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

        DisplayVisionRange(angle);
        DisplayAttackRange(angle);
        DisplayInfos();
    }
#endif
}

// Quand dans zone de trigger, approche le joueur
// Quand en range, l'attaque
// Si le joueur s'est rapproché, commence à fuir
// Si le joueur est à une distance convenable, le ré-attaque
// Sinon, au bout de 2s, se retourne et l'attaque

// Plus tard, essayer de se cacher derrière un tank