using FMODUnity;
using StateMachine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PestStateMachine : Mobs, IPest
{
    [System.Serializable]
    private class PestSounds
    {
        public EventReference deathSound;
    }

    // state machine variables
    private BaseState<PestStateMachine> currentState;
    private StateFactory<PestStateMachine> factory;

    // declare reference variables
    private EnemyLifeBar lifeBar;
    private Animator animator;

    // mobs variables
    private IAttacker.AttackDelegate onAttack;
    private IAttacker.HitDelegate onHit;
    [SerializeField] private PestSounds pestSounds;
    [SerializeField, Range(0f, 360f)] private float angle = 180.0f;
    private float searchEntityDelay = 1.0f;
    private float delayBetweenMovement = 2.0f;
    [SerializeField] private BoxCollider attackCollider;

    // animation hash
    private int chargeInHash;
    private int chargeOutHash;

    // getters and setters
    public List<Status> StatusToApply { get => statusToApply; }
    public IAttacker.AttackDelegate OnAttack { get => onAttack; set => onAttack = value; }
    public IAttacker.HitDelegate OnHit { get => onHit; set => onHit = value; }
    public BaseState<PestStateMachine> CurrentState { get => currentState; set => currentState = value; }
    public Entity[] NearbyEntities { get => nearbyEntities; }
    public float DelayBetweenMovement { get => delayBetweenMovement; }
    public Animator Animator { get => animator; }
    public NavMeshAgent Agent { get => agent; }
    public int ChargeInHash { get => chargeInHash; }
    public int ChargeOutHash { get => chargeOutHash; }
    public BoxCollider AttackCollider { get => attackCollider; }

    protected override void Start()
    {
        base.Start();

        factory = new StateFactory<PestStateMachine>(this);
        currentState = factory.GetState<PestIdleState>();

        // getter(s) reference
        lifeBar = GetComponentInChildren<EnemyLifeBar>();
        animator = GetComponentInChildren<Animator>();

        // hashing animation
        chargeInHash = Animator.StringToHash("ChargeIn");
        chargeOutHash = Animator.StringToHash("ChargeOut");
    }

    protected override void Update()
    {
        base.Update();
        currentState.Update();
    }

    #region MOBS METHODS
    protected override IEnumerator EntityDetection()
    {
        while (true)
        {
            if (!agent.enabled)
            {
                yield return null;
                continue;
            }

            nearbyEntities = PhysicsExtensions.OverlapVisionCone(transform.position, angle, (int)stats.GetValue(Stat.VISION_RANGE), transform.forward, LayerMask.GetMask("Entity"))
                    .Select(x => x.GetComponent<Entity>())
                    .Where(x => x != null && x != this)
                    .OrderBy(x => Vector3.Distance(x.transform.position, transform.position))
                    .ToArray();

            yield return new WaitForSeconds(searchEntityDelay);
        }
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

    public void Attack(IDamageable damageable)
    {
        int damages = (int)(stats.GetValue(Stat.ATK) * stats.GetValue(Stat.ATK_COEFF));
        onHit?.Invoke(damageable);
        damageable.ApplyDamage(damages);

        Knockback knockbackable = (damageable as MonoBehaviour).GetComponent<Knockback>();
        if (knockbackable)
        {
            Vector3 damageablePos = (damageable as MonoBehaviour).transform.position;
            Vector3 force = new Vector3(damageablePos.x - transform.position.x, 0f, damageablePos.z - transform.position.z).normalized;
            knockbackable.GetKnockback(force * stats.GetValue(Stat.KNOCKBACK_COEFF));
            FloatingTextGenerator.CreateActionText((damageable as MonoBehaviour).transform.position, "Pushed!");
        }
    }

    public void Death()
    {
        OnDeath?.Invoke(transform.position);
        Destroy(gameObject);
        GameObject.FindWithTag("Player").GetComponent<Hero>().OnKill?.Invoke(this);
        AudioManager.Instance.PlaySound(pestSounds.deathSound);
    }

    public void MoveTo(Vector3 posToMove)
    {
        agent.SetDestination(posToMove);
    }
    #endregion

    #region EDITOR
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
    #endregion
}
