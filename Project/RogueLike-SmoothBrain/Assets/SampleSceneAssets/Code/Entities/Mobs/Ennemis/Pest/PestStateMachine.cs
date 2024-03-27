using FMODUnity;
using StateMachine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
using UnityEngine.AI;

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
    [SerializeField] private BoxCollider attackCollider;
    private Transform target;
    private int frameToUpdate;
    private int maxFrameUpdate = 500;

    // animation hash
    private int chargeInHash;
    private int chargeOutHash;

    // getters and setters
    public List<Status> StatusToApply { get => statusToApply; }
    public IAttacker.AttackDelegate OnAttack { get => onAttack; set => onAttack = value; }
    public IAttacker.HitDelegate OnHit { get => onHit; set => onHit = value; }
    public BaseState<PestStateMachine> CurrentState { get => currentState; set => currentState = value; }
    public Entity[] NearbyEntities { get => nearbyEntities; }
    public Animator Animator { get => animator; }
    public NavMeshAgent Agent { get => agent; }
    public int ChargeInHash { get => chargeInHash; }
    public int ChargeOutHash { get => chargeOutHash; }
    public BoxCollider AttackCollider { get => attackCollider; }
    public Transform Target { get => target; set => target = value; }

    protected override void Start()
    {
        base.Start();

        factory = new StateFactory<PestStateMachine>(this);
        currentState = factory.GetState<PestPatrolState>();

        // getter(s) reference
        lifeBar = GetComponentInChildren<EnemyLifeBar>();
        animator = GetComponentInChildren<Animator>();

        // common initialization
        lifeBar.SetMaxValue(stats.GetValue(Stat.HP));

        // hashing animation
        chargeInHash = Animator.StringToHash("ChargeIn");
        chargeOutHash = Animator.StringToHash("ChargeOut");

        // opti variables
        frameToUpdate = entitySpawn % maxFrameUpdate;
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

            Entity targetE = nearbyEntities.FirstOrDefault(x => x.GetComponent<Hero>());
            if (targetE != null)
                target = targetE.transform;

            yield return new WaitUntil(() => Time.frameCount % maxFrameUpdate == frameToUpdate);
        }
    }

    public void ApplyDamage(int _value, bool isCrit = false, bool hasAnimation = true)
    {
        // Some times, this method is call when entity is dead ??
        if (stats.GetValue(Stat.HP) <= 0)
            return;

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
        ApplyKnockback(damageable);
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
        if (!agent.enabled)
            return;

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

    protected override void DisplayInfos()
    {
        Handles.Label(
        transform.position + transform.up,
        stats.GetEntityName() +
        "\n - Health : " + stats.GetValue(Stat.HP) +
        "\n - Speed : " + stats.GetValue(Stat.SPEED) +
        "\n - State : " + currentState?.ToString(),
        new GUIStyle()
        {
            alignment = TextAnchor.MiddleLeft,
            normal = new GUIStyleState()
            {
                textColor = Color.black
            }
        });
    }
#endif
    #endregion
}
