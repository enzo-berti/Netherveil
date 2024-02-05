using System.Collections;
using System.Linq;
using UnityEngine;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Range : Mobs, IDamageable, IAttacker, IMovable, IBlastable
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

    private float fleeTimer;
    private bool isFleeing;
    private Vector3 fleeTarget;

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

            if (fleeTimer > 0)
            {
                fleeTimer -= Time.deltaTime;
            }
            else
            {
                fleeTimer = 0;
            }

            if (player)
            {
                isFighting = true;

                if (Vector3.Distance(transform.position, player.transform.position) < (int)Stat.ATK_RANGE / 2f && fleeTimer == 0f)
                {
                    isFleeing = true;
                    fleeTarget = player.transform.position - transform.position;
                    fleeTarget.Normalize();
                    fleeTarget = player.transform.position + fleeTarget * (int)stats.GetValueStat(Stat.ATK_RANGE);
                }

                lastKnownPlayerPos = player.transform.position;
            }
            else if (isFighting)
            {
                MoveTo(lastKnownPlayerPos);
                if (!agent.hasPath)
                {
                    isFighting = false;
                }
            }

            if (isFleeing)
            {

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
        //if (Selection.activeGameObject != gameObject)
        //    return;

        DisplayVisionRange(isFighting ? 360 : angle);
        DisplayAttackRange(isFighting ? 360 : angle);
        DisplayInfos();
    }
#endif
}

// quand le joueur est trop près, le range va se réfugier derrière le tank
// quand le joueur est trop près, si aucun tank n'est à proximité il va fuir en ligne droite
// il ne le fera pas en boucle, il aura un cd sur sa fuite
// lorsqu'il est en cd, il va attaquer le joueur simplement


////// Code mort

//// Si un tank est à proximité
//if (tanks.Any())
//{
//    Vector3 playerTankVector = tanks.First().transform.position - player.transform.position;
//    playerTankVector.Normalize();

//    Vector3 targetPos = player.transform.position;

//    if (Vector3.Distance(targetPos, tanks.First().transform.position) < 2f)
//    {
//        targetPos = tanks.First().transform.position + playerTankVector * 2f;
//    }
//    else
//    {
//        targetPos = tanks.First().transform.position + playerTankVector * stats.GetValueStat(Stat.ATK_RANGE);
//    }

//    MoveTo(targetPos);
//}
//else
//{
//    MoveTo(player.transform.position);
//}