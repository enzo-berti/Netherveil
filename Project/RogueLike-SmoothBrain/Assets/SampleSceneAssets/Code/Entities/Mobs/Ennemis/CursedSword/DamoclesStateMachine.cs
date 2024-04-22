using UnityEngine;
using StateMachine; // include all script about stateMachine
using System.Collections.Generic;
using System.Collections;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DamoclesStateMachine : Mobs, IDamocles
{
    [System.Serializable]
    private class DamoclesSounds
    {
        public Sound deathSound;
        public Sound takeDamageSound;
        public Sound hitSound;
        public Sound moveSound;
    }

    // state machine variables
    public BaseState<DamoclesStateMachine> currentState;
    private StateFactory<DamoclesStateMachine> factory;

    // declare reference variables
    private Animator animator;

    // mobs variables
    private IAttacker.AttackDelegate onAttack;
    private IAttacker.HitDelegate onHit;
    [SerializeField] private DamoclesSounds damoclesSounds;
    [SerializeField, Range(0f, 360f)] private float angle = 180.0f;
    [SerializeField] private BoxCollider attack1Collider;
    [SerializeField] private BoxCollider attack2Collider;
    [SerializeField] private BoxCollider attack3Collider;
    private Transform target;
    private bool isDeath = false;

    // animation hash
    private int chargeInHash;
    private int chargeOutHash;
    private int deathHash;

    // getters and setters
    public List<Status> StatusToApply { get => statusToApply; }
    public IAttacker.AttackDelegate OnAttack { get => onAttack; set => onAttack = value; }
    public IAttacker.HitDelegate OnAttackHit { get => onHit; set => onHit = value; }
    public BaseState<DamoclesStateMachine> CurrentState { get => currentState; set => currentState = value; }
    public Entity[] NearbyEntities { get => nearbyEntities; }
    public Animator Animator { get => animator; }
    public BoxCollider Attack1Collider { get => attack1Collider; }
    public BoxCollider Attack2Collider { get => attack2Collider; }
    public BoxCollider Attack3Collider { get => attack3Collider; }
    public Transform Target { get => target; set => target = value; }
    public float NormalSpeed { get => Stats.GetValue(Stat.SPEED) / 10.0f; }
    public float DashSpeed { get => Stats.GetValue(Stat.SPEED) * 1.2f; }
    public bool IsDeath { get => isDeath; }


    protected override void Start()
    {
        base.Start();

        factory = new StateFactory<DamoclesStateMachine>(this);
        // Set currentState here !
        currentState = factory.GetState<DamoclesIdle>();

        // getter(s) reference
        animator = GetComponentInChildren<Animator>();

        // common initialization


        // hashing animation
        chargeInHash = Animator.StringToHash("ChargeIn");
        chargeOutHash = Animator.StringToHash("ChargeOut");
        deathHash = Animator.StringToHash("IsDeath");

        // opti variables
        maxFrameUpdate = 10;
        frameToUpdate = entitySpawn % maxFrameUpdate;
    }

    protected override void Update()
    {
        if (animator.speed == 0)
            return;

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

    public void ApplyDamage(int _value, IAttacker attacker, bool notEffectDamage = true)
    {
        ApplyDamagesMob(_value, damoclesSounds.hitSound, Death, notEffectDamage);
    }

    public void Attack(IDamageable damageable, int additionalDamages = 0)
    {
        int damages = (int)stats.GetValue(Stat.ATK);
        damages += additionalDamages;

        onHit?.Invoke(damageable, this);
        damageable.ApplyDamage(damages, this);
        ApplyKnockback(damageable, this);

        damoclesSounds.hitSound.Play(transform.position);
    }

    public void Death()
    {
        OnDeath?.Invoke(transform.position);
        Hero.OnKill?.Invoke(this);
        damoclesSounds.deathSound.Play(transform.position);
        animator.SetBool(deathHash, true);
        isDeath = true;

        Destroy(transform.parent.gameObject, animator.GetCurrentAnimatorStateInfo(0).length);
    }

    public void MoveTo(Vector3 posToMove)
    {
        if (!agent.enabled)
            return;

        agent.SetDestination(posToMove);
        damoclesSounds.moveSound.Play(transform.position);
    }

    public void Move(Vector3 direction)
    {
        if (!agent.enabled)
            return;

        agent.Move(direction);
        damoclesSounds.moveSound.Play(transform.position);
    }
    #endregion

    #region EDITOR
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        //if (!Selection.Contains(gameObject))
        //    return;

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
