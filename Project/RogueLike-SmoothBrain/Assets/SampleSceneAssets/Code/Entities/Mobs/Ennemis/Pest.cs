using System.Collections;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using FMODUnity;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Pest : Mobs, IAttacker, IDamageable, IMovable, IBlastable
{
    // think useless
    private IAttacker.AttackDelegate onAttack;
    private IAttacker.HitDelegate onHit;
    private List<Status> statusToApply = new List<Status>();
    public IAttacker.AttackDelegate OnAttack { get => onAttack; set => onAttack = value; }
    public IAttacker.HitDelegate OnHit { get => onHit; set => onHit = value; }
    public List<Status> StatusToApply { get => statusToApply; }

    // gameobjects & components
    private Animator animator;
    private EnemyLifeBar lifeBar;

    // pest parameters
    [Header("Pest Parameters")]
    [SerializeField, Range(0f, 360f)] private float angle = 120f;
    [SerializeField] private float brainDelay = 1f;
     
    // audio
    [Header("Pest audio")]
    [SerializeField] private EventReference deathSound;

    // animation hashing
    private int movingTriggerHash;

    protected override void Start()
    {
        base.Start();

        // component initialization
        animator = GetComponentInChildren<Animator>();
        lifeBar = GetComponentInChildren<EnemyLifeBar>();

        // animator hashing
        movingTriggerHash = Animator.StringToHash("MovingTrigger");

        // common initialization
        lifeBar.SetMaxValue(stats.GetValue(Stat.HP));
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
                    averagePos += pest.transform.position * Random.Range(0.5f, 1.5f);
                }
                averagePos /= pests.Count();

                Vector3 avoidPos = Vector3.zero;
                foreach (Pest pest in pests)
                {
                    avoidPos += (pest.transform.position - transform.position).normalized * (1 - Vector3.Distance(transform.position, pest.transform.position)) * Random.Range(0.5f, 1.5f);
                }
                avoidPos /= pests.Count();
                 
                MoveTo(averagePos + avoidPos);
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
        int damages = (int)(stats.GetValue(Stat.ATK) * stats.GetValue(Stat.ATK_COEFF));
        onHit?.Invoke(damageable);
        damageable.ApplyDamage(damages);
    }

    public void ApplyDamage(int _value, bool isCrit = false, bool hasAnimation = true)
    {
        Stats.IncreaseValue(Stat.HP, -_value, false);
        lifeBar.ValueChanged(stats.GetValue(Stat.HP));
        
        if (hasAnimation)
        {
            //add SFX here
            FloatingTextGenerator.CreateDamageText(_value, transform.position, isCrit);
            StartCoroutine(HitRoutine());
        }

        if (stats.GetValue(Stat.HP) <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        OnDeath?.Invoke(transform.position);
        Destroy(gameObject);
        GameObject.FindWithTag("Player").GetComponent<Hero>().OnKill?.Invoke(this);
        AudioManager.Instance.PlaySound(deathSound);
    }

    public void MoveTo(Vector3 posToMove)
    {
        agent.SetDestination(posToMove);
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