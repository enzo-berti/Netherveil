using System.Collections;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Pest : Mobs, IAttacker, IDamageable, IMovable, IKnockbackable, IBlastable
{
    private IAttacker.AttackDelegate onAttack;
    private IAttacker.HitDelegate onHit;
    public IAttacker.AttackDelegate OnAttack { get => onAttack; set => onAttack = value; }
    public IAttacker.HitDelegate OnHit { get => onHit; set => onHit = value; }

    [Header("Pest Parameters")]
    [SerializeField, Range(0f, 360f)] private float angle = 120f;
    [SerializeField, Range(0.001f, 0.1f)] private float StillThreshold = 0.05f;
    [SerializeField] private float movementDelay = 2f;
    [SerializeField] private float attackDelay = 2f;
    private Coroutine attackRoutine;
    private Coroutine knockbackRoutine;

    private void OnCollisionEnter(Collision collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

        if (!collision.gameObject.CompareTag("Player") || damageable == null || attackRoutine != null)
            return;

        attackRoutine = StartCoroutine(ApplyAttack(damageable));
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player") || attackRoutine == null)
            return;

        StopCoroutine(attackRoutine);
    }

    protected override IEnumerator EntityDetection()
    {
        while (true)
        {
            yield return new WaitForSeconds(movementDelay);

            if (!agent.enabled)
                continue;

            nearbyEntities = PhysicsExtensions.OverlapVisionCone(transform.position, angle, (int)stats.GetValueStat(Stat.VISION_RANGE), transform.forward, LayerMask.GetMask("Entity"))
                    .Select(x => x.GetComponent<Entity>())
                    .Where(x => x != null && x != this)
                    .OrderBy(x => Vector3.Distance(x.transform.position, transform.position))
                    .ToArray();
        }
    }

    protected override IEnumerator Brain()
    {
        while (true)
        {
            yield return new WaitForSeconds(movementDelay);

            if (!agent.enabled)
                continue;

            Hero player = nearbyEntities
                .Select(x => x.GetComponent<Hero>())
                .Where(x => x != null)
                .FirstOrDefault();

            Pest[] pests = nearbyEntities
                .Select(x => x.GetComponent<Pest>())
                .Where(x => x != null)
                .ToArray();

            if (player)
            {
                // Attack Player
                if (Vector3.Distance(transform.position, player.transform.position) <= (int)stats.GetValueStat(Stat.ATK_RANGE))
                {
                    player.ApplyDamage((int)stats.GetValueStat(Stat.ATK));
                }
                // Move to Player
                else
                {
                    MoveTo(player.transform.position);
                }
            }
            else if (pests.Any())
            {
                // Other pest detect
                MoveTo(pests.First().transform.position);
            }
            else
            {
                // Random movement
                Vector2 rdmPos = Random.insideUnitCircle * (int)stats.GetValueStat(Stat.VISION_RANGE);
                MoveTo(transform.position + new Vector3(rdmPos.x, 0, rdmPos.y));
            }
        }
    }

    public void Attack(IDamageable damageable)
    {
        damageable.ApplyDamage((int)(stats.GetValueStat(Stat.ATK) * stats.GetValueStat(Stat.ATK_COEFF)));
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

    public void GetKnockback(Vector3 force)
    {
        if (knockbackRoutine != null)
            return;

        knockbackRoutine = StartCoroutine(ApplyKnockback(force));
    }

    public void MoveTo(Vector3 posToMove)
    {
        agent.SetDestination(posToMove);
    }

    private IEnumerator ApplyAttack(IDamageable damageable)
    {
        while (true)
        {
            Attack(damageable);
            yield return new WaitForSeconds(attackDelay);
        }
    }

    protected IEnumerator ApplyKnockback(Vector3 force)
    {
        yield return null;
        agent.enabled = false;
        rb.isKinematic = false;
        rb.AddForce(force, ForceMode.Impulse);

        yield return new WaitForFixedUpdate();
        yield return new WaitUntil(() => rb.velocity.magnitude < StillThreshold);
        yield return new WaitForSeconds(0.25f);

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        agent.Warp(transform.position);
        agent.enabled = true;

        knockbackRoutine = null;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!Selection.Contains(gameObject))
            return;

        DisplayVisionRange(angle);
        DisplayAttackRange(angle);
        DisplayInfos();
    }
#endif
}


// |--------------|
// |PEST BEHAVIOUR|
// |--------------|
// If Player detect
//  Attack Player
// Else if Pest detect
//  Follow Pest
// Else
//  Random jump