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
    private IAttacker.AttackDelegate onAttack;
    private IAttacker.HitDelegate onHit;
    private List<Status> statusToApply = new List<Status>();
    public IAttacker.AttackDelegate OnAttack { get => onAttack; set => onAttack = value; }
    public IAttacker.HitDelegate OnHit { get => onHit; set => onHit = value; }
    public List<Status> StatusToApply { get => statusToApply; }

    [SerializeField] Slider lifebar;
    [SerializeField] Slider damageBar;

    [Header("Pest Parameters")]
    [SerializeField, Range(0f, 360f)] private float angle = 120f;
    [SerializeField] private float brainDelay = 2f;

    [Header("Pest audio")]
    [SerializeField] private EventReference deathSound;

    private SkinnedMeshRenderer skinnedMeshRenderer;

    // Animator
    private Animator animator;
    private int movingTriggerHash;

    protected override void Start()
    {
        base.Start();

        animator = GetComponentInChildren<Animator>();
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        movingTriggerHash = Animator.StringToHash("MovingTrigger");

        InitLifeBar();
    }

    private void InitLifeBar()
    {
        lifebar.maxValue = stats.GetValue(Stat.HP);
        damageBar.maxValue = stats.GetValue(Stat.HP);
        lifebar.value = lifebar.maxValue;
        damageBar.value = damageBar.maxValue;
        Vector2 size = lifebar.transform.parent.GetComponent<RectTransform>().sizeDelta;
        size.x *= stats.GetValue(Stat.HP) / 100;
        size.x = Mathf.Clamp(size.x, 100f, 300f);
        lifebar.transform.parent.GetComponent<RectTransform>().sizeDelta = size;
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
        FloatingTextGenerator.CreateDamageText(damages, (damageable as MonoBehaviour).transform.position);
    }

    public void ApplyDamage(int _value, bool hasAnimation = true)
    {
        Stats.IncreaseValue(Stat.HP, -_value, false);
        lifebar.value = stats.GetValue(Stat.HP);
        if (hasAnimation)
        {
            //add SFX here
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

    private IEnumerator HitRoutine()
    {
        skinnedMeshRenderer.material.SetInt("_isHit", 1);
        yield return new WaitForSeconds(0.05f);
        skinnedMeshRenderer.material.SetInt("_isHit", 0);
        yield return new WaitForSeconds(0.05f);
        skinnedMeshRenderer.material.SetInt("_isHit", 1);
        yield return new WaitForSeconds(0.05f);
        skinnedMeshRenderer.material.SetInt("_isHit", 0);
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