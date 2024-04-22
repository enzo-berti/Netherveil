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
using UnityEngine.VFX;

public class GorgonStateMachine : Mobs, IGorgon
{
    [Serializable]
    public class GorgonSounds
    {
        public Sound hitSFX;
        public Sound deathSFX;
    }

    // state machine variables
    [HideInInspector]
    public BaseState<GorgonStateMachine> currentState;
    private StateFactory<GorgonStateMachine> factory;

    // mob variables
    private IAttacker.HitDelegate onHit;
    private IAttacker.AttackDelegate onAttack;
    [Header("Range Parameters")]
    [SerializeField] private float timeBetweenAttack;
    [SerializeField] private float timeBetweenFleeing;
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private Transform hand;
    [SerializeField] float defaultVisionAngle = 145f;
    public VisualEffect dashVFX;
    [SerializeField] GorgonSounds gorgonSounds;

    Hero player = null;

    ///
    private bool isDashing = false;
    private bool isSmoothCoroutineOn = false;
    private bool hasLaunchAnim = false;
    private bool hasRemoveHead = false;

    bool canLoseAggro = true;
    float attackCooldown = 0f;
    float MAX_ATTACK_COOLDOWN = 1f;

    float dashCooldown = 0f;
    float MAX_DASH_COOLDOWN = 2f;

    float fleeCooldown = 0f;
    float MAX_FLEE_COOLDOWN = 1f;

    #region getters/setters
    public IAttacker.HitDelegate OnAttackHit { get => onHit; set => onHit = value; }
    public IAttacker.AttackDelegate OnAttack { get => onAttack; set => onAttack = value; }
    public Hero Player { get => player; }
    public Animator Animator { get => animator; }
    public bool HasLaunchAnim { get => hasLaunchAnim; set => hasLaunchAnim = value; }
    public bool HasRemovedHead { get => hasRemoveHead; set => hasRemoveHead = value; }
    //private float DistanceToFlee { get => stats.GetValue(Stat.ATK_RANGE) / 1.5f; }
    public GameObject BombPrefab { get => bombPrefab; }
    public Transform Hand { get => hand; }
    public float VisionAngle { get => !canLoseAggro ? 360 : (currentState is GorgonTriggeredState || currentState is GorgonAttackingState) && player != null ? 360 : defaultVisionAngle; }
    public float VisionRange { get => !canLoseAggro ? Stats.GetValue(Stat.VISION_RANGE) * 1.25f : Stats.GetValue(Stat.VISION_RANGE) * (currentState is GorgonTriggeredState || currentState is GorgonAttackingState ? 1.25f : 1f); }
    public bool CanLoseAggro { set => canLoseAggro = value; }
    public bool IsAttackAvailable { get => attackCooldown >= MAX_ATTACK_COOLDOWN; }
    public float AttackCooldown { get => attackCooldown; set => attackCooldown = value; }
    public bool IsDashAvailable { get => dashCooldown >= MAX_DASH_COOLDOWN; }
    public float DashCooldown { get => dashCooldown; set => dashCooldown = value; }
    public bool IsFleeAvailable { get => fleeCooldown >= MAX_FLEE_COOLDOWN; }
    public float FleeCooldown { get => fleeCooldown; set => fleeCooldown = value; }
    public float TimeBetweenAttacks { get => timeBetweenAttack; }
    public bool IsDashing { get => isDashing; }
    #endregion

    public List<Status> StatusToApply { get => statusToApply; }

    protected override void Start()
    {
        base.Start();

        factory = new StateFactory<GorgonStateMachine>(this);
        currentState = factory.GetState<GorgonWanderingState>();

        // opti variables
        maxFrameUpdate = 10;
        frameToUpdate = entitySpawn % maxFrameUpdate;
    }

    protected override void Update()
    {
        if (animator.speed == 0)
            return;

        base.Update();

        if (currentState is not GorgonAttackingState)
            if (attackCooldown < MAX_ATTACK_COOLDOWN) attackCooldown += Time.deltaTime;

        if (currentState is not GorgonDashingState)
            if (dashCooldown < MAX_DASH_COOLDOWN) dashCooldown += Time.deltaTime;

        if (currentState is not GorgonFleeingState)
            if (fleeCooldown < MAX_FLEE_COOLDOWN) fleeCooldown += Time.deltaTime;

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

            if (!canLoseAggro)
            {
                player = Utilities.Hero;
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
        ApplyDamagesMob(_value, gorgonSounds.hitSFX, Death, notEffectDamage);

        if (!player)
        {
            currentState = factory.GetState<GorgonTriggeredState>();
            player = Utilities.Hero;
        }
    }

    public void Death()
    {
        OnDeath?.Invoke(transform.position);
        Hero.OnKill?.Invoke(this);

        gorgonSounds.deathSFX.Play(transform.position);

        Destroy(transform.parent.gameObject);
    }

    public void Attack(IDamageable damageable, int additionalDamages = 0)
    {
        int damages = (int)stats.GetValue(Stat.ATK);
        damages += additionalDamages;
        onHit?.Invoke(damageable, this);
        damageable.ApplyDamage(damages, this);
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

    #region Extra methods

    public IEnumerator DashToPos(List<Vector3> listDashes)
    {
        isDashing = true;

        for (int i = 1; i < listDashes.Count; i++)
        {
            if (this.Stats.GetValue(Stat.SPEED) > 0)
            {
                StartCoroutine(GoSmoothToPosition(listDashes[i]));
                animator.ResetTrigger("Dash");
                animator.SetTrigger("Dash");
            }
            yield return new WaitUntil(() => isSmoothCoroutineOn == false);
        }

        isDashing = false;
    }

    private IEnumerator GoSmoothToPosition(Vector3 posToReach)
    {
        isSmoothCoroutineOn = true;

        float timer = 0;
        Vector3 basePos = this.transform.position;
        Vector3 newPos;
        // Face to his next direction
        this.transform.forward = posToReach - basePos;
        while (timer < 1f)
        {
            newPos = Vector3.Lerp(basePos, posToReach, timer);
            agent.Warp(newPos);
            timer += Time.deltaTime * 5;
            timer = timer > 1 ? 1 : timer;
            yield return null;
        }
        yield return new WaitForSeconds(0.25f);

        isSmoothCoroutineOn = false;
    }

    #endregion
}