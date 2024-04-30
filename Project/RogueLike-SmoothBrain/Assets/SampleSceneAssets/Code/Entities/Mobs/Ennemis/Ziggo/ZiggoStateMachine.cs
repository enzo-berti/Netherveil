using UnityEngine;
using StateMachine; // include all script about stateMachine
using System.Collections.Generic;
using System.Collections;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ZiggoStateMachine : Mobs, IZiggo
{
    [System.Serializable]
    private class ZiggoSounds
    {
        public Sound deathSound;
        public Sound takeDamageSound;
        public Sound hitSound;
        public Sound moveSound;
    }

    // state machine variables
    [HideInInspector] public BaseState<ZiggoStateMachine> currentState;
    private StateFactory<ZiggoStateMachine> factory;

    // mobs variables
    private IAttacker.AttackDelegate onAttack;
    private IAttacker.HitDelegate onHit;
    [SerializeField] private ZiggoSounds ziggoSounds;
    [SerializeField, Range(0f, 360f)] private float originalVisionAngle = 180.0f;
    [SerializeField] private BoxCollider attack1Collider;
    [SerializeField] private BoxCollider attack2Collider;
    [SerializeField] private BoxCollider attack3Collider;
    private Hero player;

    // animation hash
    private int deathHash;

    // attacks
    float dashCooldown = 0f;
    float spitCooldown = 0f;

    #region Getters/setters
    public List<Status> StatusToApply { get => statusToApply; }
    public IAttacker.AttackDelegate OnAttack { get => onAttack; set => onAttack = value; }
    public IAttacker.HitDelegate OnAttackHit { get => onHit; set => onHit = value; }
    public BaseState<ZiggoStateMachine> CurrentState { get => currentState; set => currentState = value; }
    public Entity[] NearbyEntities { get => nearbyEntities; }
    public Animator Animator { get => animator; }
    public BoxCollider Attack1Collider { get => attack1Collider; }
    public BoxCollider Attack2Collider { get => attack2Collider; }
    public BoxCollider Attack3Collider { get => attack3Collider; }
    public Hero Player { get => player; }
    public float VisionRange { get => stats.GetValue(Stat.VISION_RANGE) * (currentState is not ZiggoWanderingState ? 1.25f : 1f); }
    public float VisionAngle { get => player ? 360 : originalVisionAngle; }
    public float DashCooldown { get => dashCooldown; set => dashCooldown = value; }
    public float SpitCooldown { get => spitCooldown; set => spitCooldown = value; }
    #endregion

    protected override void Start()
    {
        base.Start();

        factory = new StateFactory<ZiggoStateMachine>(this);
        currentState = factory.GetState<ZiggoWanderingState>();

        // common initialization


        // hashing animation


        // opti variables
        maxFrameUpdate = 10;
        frameToUpdate = entitySpawn % maxFrameUpdate;
    }

    protected override void Update()
    {
        if (isFreeze || IsSpawning)
            return;

        base.Update();

        currentState.Update();

        if (currentState is not ZiggoWanderingState)
            WanderZoneCenter = transform.position;
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

            nearbyEntities = PhysicsExtensions.OverlapVisionCone(transform.position, VisionAngle, VisionRange, transform.forward, LayerMask.GetMask("Entity"))
                    .Select(x => x.GetComponent<Hero>())
                    .Where(x => x != null && x != this)
                    .OrderBy(x => Vector3.Distance(x.transform.position, transform.position))
                    .ToArray();

            Entity playerEntity = nearbyEntities.FirstOrDefault(x => x.GetComponent<Hero>());
            player = playerEntity != null ? playerEntity.GetComponent<Hero>() : null;

            yield return new WaitUntil(() => Time.frameCount % maxFrameUpdate == frameToUpdate);
        }
    }

    public void ApplyDamage(int _value, IAttacker attacker, bool notEffectDamage = true)
    {
        ApplyDamagesMob(_value, ziggoSounds.hitSound, Death, notEffectDamage);
    }

    public void Attack(IDamageable damageable, int additionalDamages = 0)
    {
        int damages = (int)stats.GetValue(Stat.ATK);
        damages += additionalDamages;

        onHit?.Invoke(damageable, this);
        damageable.ApplyDamage(damages, this);
        ApplyKnockback(damageable, this);

        ziggoSounds.hitSound.Play(transform.position);
    }

    public void Death()
    {
        OnDeath?.Invoke(transform.position);
        Hero.OnKill?.Invoke(this);
        ziggoSounds.deathSound.Play(transform.position);
        animator.SetBool(deathHash, true);

        currentState = factory.GetState<ZiggoDeathState>();

        Destroy(transform.parent.gameObject, animator.GetCurrentAnimatorStateInfo(0).length);
    }

    public void MoveTo(Vector3 posToMove)
    {
        if (!agent.enabled)
            return;

        agent.SetDestination(posToMove);
        ziggoSounds.moveSound.Play(transform.position);
    }

    public void Move(Vector3 direction)
    {
        if (!agent.enabled)
            return;

        agent.Move(direction);
        ziggoSounds.moveSound.Play(transform.position);
    }
    #endregion

    #region EDITOR
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        //if (!Selection.Contains(gameObject))
        //    return;

        DisplayVisionRange(VisionAngle, VisionRange);
        DisplayAttackRange(VisionAngle);
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
