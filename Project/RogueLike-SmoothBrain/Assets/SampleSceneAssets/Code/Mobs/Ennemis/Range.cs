using System.Collections;
using System.Linq;
using UnityEngine;
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

            Tank[] tanks = entities
                .Select(x => x.GetComponent<Tank>())
                .Where(x => x != null)
                .OrderBy(x => Vector3.Distance(x.transform.position, transform.position))
                .ToArray();

            // Update timer fuite
            fleeTimer = fleeTimer > 0 ? fleeTimer -= Time.deltaTime : 0;

            if (player)
            {
                isFighting = true;

                if (!isFleeing) MoveTo(player.transform.position);

                // si le joueur est trop près et que la fuite est dispo
                if (Vector3.Distance(transform.position, player.transform.position) < (int)stats.GetValueStat(Stat.ATK_RANGE) / 2f && fleeTimer == 0f)
                {
                    isFleeing = true;
                    fleeTimer = 4f;

                    // si tanks à proximité
                    if (tanks.Any() && Vector3.Distance(player.transform.position, tanks.First().transform.position) + 2f < stats.GetValueStat(Stat.VISION_RANGE))
                    {
                        Vector3 playerToTankVector = tanks.First().transform.position - player.transform.position;
                        playerToTankVector.Normalize();

                        fleeTarget = tanks.First().transform.position + playerToTankVector * 2f;
                    }
                    else
                    {
                        Vector3 playerToEnemyVector = transform.position - player.transform.position;
                        playerToEnemyVector.Normalize();

                        fleeTarget = player.transform.position + playerToEnemyVector * (int)stats.GetValueStat(Stat.ATK_RANGE);
                    }

                    MoveTo(fleeTarget);
                }
            }

            if (!agent.hasPath)
            {
                if (isFleeing)
                {
                    isFleeing = false;
                }
                else if (isFighting)
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
        GameObject.FindWithTag("Player").GetComponent<Hero>().OnKill?.Invoke(this);
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