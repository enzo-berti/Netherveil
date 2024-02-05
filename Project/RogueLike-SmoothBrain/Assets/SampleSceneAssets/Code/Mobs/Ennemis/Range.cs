using System.Collections;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Range : Mobs, IDamageable, IAttacker, IMovable
{
    private enum RangeState
    {
        WANDERING,
        TRIGGERED,
        STAGGERED,
        CHARGING
    }

    private IAttacker.HitDelegate onHit;
    private IAttacker.AttackDelegate onAttack;
    public IAttacker.HitDelegate OnHit { get => onHit; set => onHit = value; }
    public IAttacker.AttackDelegate OnAttack { get => onAttack; set => onAttack = value; }

    [Header("Range Parameters")]
    [SerializeField, Range(0f, 360f)] private float angle = 120f;
    [SerializeField, Min(0)] private float staggerDuration;

    private bool isFighting = false;
    private RangeState state;
    Vector3 lastKnownPlayerPos;

    private float staggerImmunity;
    private float staggerTimer;

    protected override IEnumerator Brain()
    {
        while (true)
        {
            yield return null;

            Entity[] entities = PhysicsExtensions.OverlapVisionCone(transform.position, isFighting ? 360 : angle, (int)stats.GetValueStat(Stat.VISION_RANGE), transform.forward, LayerMask.GetMask("Entity"))
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
                isFighting = true;
                lastKnownPlayerPos = player.transform.position;

                // Si un tank est à proximité
                if (tanks.Any())
                {
                    Vector3 playerTankVector = tanks.First().transform.position - player.transform.position;

                    playerTankVector.Normalize();
                    Vector3 targetPos = player.transform.position + playerTankVector * stats.GetValueStat(Stat.ATK_RANGE);

                    MoveTo(targetPos);
                }
                else
                {
                    MoveTo(player.transform.position);
                }
            }
            else if (isFighting)
            {
                MoveTo(lastKnownPlayerPos);
                if (!agent.hasPath)
                {
                    isFighting = false;
                }
            }

            UpdateStates();
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
    void UpdateStates()
    {
        if (isFighting)
        {
            state = RangeState.TRIGGERED;
        }
        else
        {
            state = RangeState.WANDERING;
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
        if (Selection.activeGameObject != gameObject)
            return;

        DisplayVisionRange(isFighting ? 360 : angle);
        DisplayAttackRange(isFighting ? 360 : angle);
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