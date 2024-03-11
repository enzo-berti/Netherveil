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

    private float FleeingRange => (int)stats.GetValue(Stat.ATK_RANGE) / 2f;

    protected override IEnumerator EntityDetection()
    {
        while (true)
        {
            if (!agent.enabled)
                continue;

            nearbyEntities = PhysicsExtensions.OverlapVisionCone(transform.position, isFighting ? 360 : angle, (int)stats.GetValue(Stat.VISION_RANGE), transform.forward, LayerMask.GetMask("Entity"))
                    .Select(x => x.GetComponent<Entity>())
                    .Where(x => x != null && x != this)
                    .OrderBy(x => Vector3.Distance(x.transform.position, transform.position))
                    .ToArray();

            yield return new WaitForSeconds(0.5f);
        }
    }

    protected override IEnumerator Brain()
    {
        while (true)
        {
            yield return null;

            if (!agent.enabled)
                continue;

            Hero player = null;
            Tank[] tanks = null;
            if (nearbyEntities.Length > 0)
            {
                player = nearbyEntities
                    .Select(x => x.GetComponent<Hero>())
                    .Where(x => x != null)
                    .FirstOrDefault();

                tanks = nearbyEntities
                    .Select(x => x.GetComponent<Tank>())
                    .Where(x => x != null)
                    .OrderBy(x => Vector3.Distance(x.transform.position, transform.position))
                    .ToArray();
            }

            // Update timer fuite
            fleeTimer = fleeTimer > 0 ? fleeTimer -= Time.deltaTime : 0;

            if (player)
            {
                isFighting = true;
                float distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);

                // si le joueur est trop près et que la fuite est dispo
                if (distanceFromPlayer < FleeingRange && fleeTimer == 0f)
                {
                    isFleeing = true;
                    fleeTimer = 4f;

                    // si tanks à proximité
                    if (tanks.Any() && Vector3.Distance(player.transform.position, tanks.First().transform.position) + 2f < stats.GetValue(Stat.VISION_RANGE))
                    {
                        Vector3 playerToTankVector = tanks.First().transform.position - player.transform.position;
                        playerToTankVector.Normalize();

                        fleeTarget = tanks.First().transform.position + playerToTankVector * 2f;
                    }
                    else
                    {
                        Vector3 playerToEnemyVector = transform.position - player.transform.position;
                        playerToEnemyVector.Normalize();

                        fleeTarget = player.transform.position + playerToEnemyVector * (int)stats.GetValue(Stat.ATK_RANGE);
                    }

                    MoveTo(fleeTarget);
                }
                // sinon si dans la zone d'attaque, attaquer
                else if (distanceFromPlayer < stats.GetValue(Stat.ATK_RANGE))
                {
                    // Attacker
                }
                // sinon si pas en fuite, avancer vers le joueur
                else if (!isFleeing)
                {
                    Vector3 direction = (player.transform.position - transform.position).normalized;
                    float factor = (int)stats.GetValue(Stat.VISION_RANGE) - (int)stats.GetValue(Stat.ATK_RANGE);
                    MoveTo(transform.position + direction * factor);
                }
            }
            else if (!agent.hasPath)
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
        Stats.IncreaseValue(Stat.HP, -_value, false);
        FloatingTextGenerator.CreateDamageText(_value, transform.position + Vector3.up * 2, false, 1);

        if (stats.GetValue(Stat.HP) <= 0)
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
        damageable.ApplyDamage((int)(stats.GetValue(Stat.ATK) * stats.GetValue(Stat.ATK_COEFF)));
    }

    public void MoveTo(Vector3 posToMove)
    {
        agent.SetDestination(posToMove);
    }

#if UNITY_EDITOR
    private void DisplayFleeingRange(float _angle)
    {
        Handles.color = new Color(1, 1, 0f, 0.25f);
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, _angle / 2f, FleeingRange);
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -_angle / 2f, FleeingRange);

        Handles.color = Color.white;
        Handles.DrawWireDisc(transform.position, Vector3.up, FleeingRange);
    }

    private void OnDrawGizmos()
    {
        if (!Selection.Contains(gameObject))
            return;

        DisplayVisionRange(isFighting ? 360 : angle);
        DisplayAttackRange(isFighting ? 360 : angle);
        DisplayFleeingRange(isFighting ? 360 : angle);
        DisplayInfos();
    }
#endif
}

// quand le joueur est trop près, le range va se réfugier derrière le tank
// quand le joueur est trop près, si aucun tank n'est à proximité il va fuir en ligne droite
// il ne le fera pas en boucle, il aura un cd sur sa fuite
// lorsqu'il est en cd, il va attaquer le joueur simplement -> TODO