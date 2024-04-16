// ---[ STATE MACHINE ] ---
// "factory" is use to get all state possible
// "currentState" can be set in the start with : currentState = factory.GetState<YOUR_STATE>();

using StateMachine; // include all script about stateMachine
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GlorbStateMachine : Mobs, IGlorb
{
    [Serializable]
    class GlorbSounds
    {
        public Sound shockwaveSFX;
        public Sound punchSFX;
        public Sound hitSFX;
        public Sound deathSFX;
    }

    // state machine variables
    [HideInInspector]
    public BaseState<GlorbStateMachine> currentState;
    private StateFactory<GlorbStateMachine> factory;

    // mob parameters
    private IAttacker.AttackDelegate onAttack;
    private IAttacker.HitDelegate onHit;
    [SerializeField] GlorbSounds glorbSounds;
    [SerializeField] CapsuleCollider shockwaveCollider;
    [SerializeField] float defaultVisionAngle = 100f;

    VFXStopper vfxStopper;
    Animator animator;
    Hero player = null;

    bool cooldownSpeAttack = false;
    float specialAttackTimer = 0f;
    readonly float SPECIAL_ATTACK_TIMER = 2.2f;

    bool cooldownBasicAttack = false;
    float basicAttackTimer = 0f;
    readonly float BASIC_ATTACK_TIMER = 0.75f;

    // animation Hash
    int deathHash;

    // Getters/Setters
    public List<Status> StatusToApply { get => statusToApply; }
    public IAttacker.AttackDelegate OnAttack { get => onAttack; set => onAttack = value; }
    public IAttacker.HitDelegate OnAttackHit { get => onHit; set => onHit = value; }
    public Hero Player { get => player; }
    public float VisionAngle { get => (currentState is GlorbTriggeredState || currentState is GlorbAttackingState) && Player != null ? 360 : defaultVisionAngle; }
    public float VisionRange { get => Stats.GetValue(Stat.VISION_RANGE) * (currentState is GlorbTriggeredState || currentState is GlorbAttackingState ? 1.5f : 1f); }

    protected override void Start()
    {
        base.Start();

        factory = new StateFactory<GlorbStateMachine>(this);
        currentState = factory.GetState<GlorbWanderingState>();

        player = null;
        vfxStopper = GetComponent<VFXStopper>();
        animator = GetComponentInChildren<Animator>();

        // animation Hash
        deathHash = Animator.StringToHash("Death");

        // opti variables
        maxFrameUpdate = 10;
        frameToUpdate = entitySpawn % maxFrameUpdate;
    }

    protected override void Update()
    {
        currentState.Update();
    }

    #region MOB_METHODS
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
                    .Select(x => x.GetComponent<Entity>())
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
        ApplyDamagesMob(_value, glorbSounds.hitSFX, Death, notEffectDamage);

        if (!(currentState is GlorbAttackingState || currentState is GlorbDeathState))
        {
            currentState = factory.GetState<GlorbTriggeredState>();
            player = GameObject.FindWithTag("Player").GetComponent<Hero>();
        }

    }

    public void Attack(IDamageable damageable, int additionalDamages = 0)
    {
        int damages = (int)stats.GetValue(Stat.ATK);
        damages += additionalDamages;

        onHit?.Invoke(damageable, this);
        damageable.ApplyDamage(damages, this);
        ApplyKnockback(damageable, this);

        glorbSounds.hitSFX.Play(transform.position);
    }

    public void Death()
    {
        OnDeath?.Invoke(transform.position);
        Hero.OnKill?.Invoke(this);

        glorbSounds.deathSFX.Play(transform.position);

        animator.ResetTrigger(deathHash);
        animator.SetTrigger(deathHash);

        Destroy(transform.parent.gameObject, animator.GetCurrentAnimatorStateInfo(0).length);

        currentState = factory.GetState<GlorbDeathState>();
    }

    public void MoveTo(Vector3 posToMove)
    {
        agent.SetDestination(posToMove);
        //AudioManager.Instance.PlaySound(glorbSounds.walkSFX, transform.position);
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
