using System.Collections;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Pest : Mobs, IAttacker, IDamageable, IMovable, IKnockbackable, IBlastable
{
    private IAttacker.AttackDelegate onAttack;
    private IAttacker.HitDelegate onHit;
    private List<Status> statusToApply = new List<Status>();
    public IAttacker.AttackDelegate OnAttack { get => onAttack; set => onAttack = value; }
    public IAttacker.HitDelegate OnHit { get => onHit; set => onHit = value; }
    public List<Status> StatusToApply { get => statusToApply; }

    [Header("Pest Parameters")]
    [SerializeField, Range(0f, 360f)] private float angle = 120f;
    [SerializeField, Range(0.001f, 0.1f)] private float StillThreshold = 0.05f;
    [SerializeField] private float brainDelay = 2f;
    private Coroutine knockbackRoutine;

    // Animator
    private Animator animator;
    private int movingTriggerHash;

    protected override void Start()
    {
        base.Start();
        animator = GetComponentInChildren<Animator>();

        movingTriggerHash = Animator.StringToHash("MovingTrigger");
    }

    protected override IEnumerator EntityDetection()
    {
        while (true)
        {
            yield return new WaitForSeconds(brainDelay);

            if (!agent.enabled)
                continue;

            nearbyEntities = PhysicsExtensions.OverlapVisionCone(transform.position, angle, (int)stats.GetValue(Stat.VISION_RANGE), transform.forward, LayerMask.GetMask("Entity"))
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
            yield return new WaitForSeconds(brainDelay);

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
                if (Vector3.Distance(transform.position, player.transform.position) <= (int)stats.GetValue(Stat.ATK_RANGE))
                {
                    Attack(player);
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
                Vector3 averagePos = Vector3.zero;
                foreach (Pest pest in pests)
                {
                    averagePos += pest.transform.position;
                }
                averagePos /= pests.Count();

                Vector2 rnadomCirc = Random.insideUnitCircle * 2.5f;
                Vector3 randomPos = new Vector3(rnadomCirc.x, 0, rnadomCirc.y);

                MoveTo(averagePos + randomPos);
            }
            else
            {
                // Random movement
                Vector2 rdmPos = Random.insideUnitCircle.normalized * (int)stats.GetValue(Stat.VISION_RANGE);
                MoveTo(transform.position + new Vector3(rdmPos.x, 0, rdmPos.y));
            }

            animator.ResetTrigger(movingTriggerHash);
            animator.SetTrigger(movingTriggerHash);
        }
    }

    public void Attack(IDamageable damageable)
    {
        onHit?.Invoke(damageable);
        damageable.ApplyDamage((int)(stats.GetValue(Stat.ATK) * stats.GetValue(Stat.ATK_COEFF)));
    }

    public void ApplyDamage(int _value, bool hasAnimation = true)
    {
        Stats.IncreaseValue(Stat.HP, -_value, false);
        FloatingTextGenerator.CreateDamageText(_value, transform.position);
        if (stats.GetValue(Stat.HP) <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        OnDeath?.Invoke(this.transform.position);
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