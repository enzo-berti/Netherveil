using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Grafted : Mobs, IAttacker, IDamageable, IMovable, IBlastable
{
    private IAttacker.AttackDelegate onAttack;
    private IAttacker.HitDelegate onHit;
    public IAttacker.AttackDelegate OnAttack { get => onAttack; set => onAttack = value; }
    public IAttacker.HitDelegate OnHit { get => onHit; set => onHit = value; }

    public List<Status> StatusToApply => statusToApply;

    private Animator animator;

    int dyingHash;
    int thrustHash;
    int dashHash;
    int throwingHash;
    int retrievingHash;


    enum Attacks
    {
        THRUST,
        DASH,
        AOE,
        RANGE,
        NONE
    }

    enum AttackState
    {
        CHARGING,
        ATTACKING,
        RECOVERING,
        IDLE
    }

    [System.Serializable]
    private class GraftedSounds
    {
        public EventReference deathSound;
        public EventReference hitSound;
        public EventReference plantInGroundSound;
        public EventReference projectileLaunchedSound;
        public EventReference projectileHitMapSound;
        public EventReference projectileHitPlayerSound;
        public EventReference thrustSound;
        public EventReference introSound;
        public EventReference retrievingProjectileLaunchedSound;
        public EventReference spinAttackSound;
        public EventReference stretchSound;
        public EventReference weaponOutSound;
        public EventReference weaponInSound;
        public EventReference walkingSound;
        public EventReference music;
    }
    [Header("Sounds")]
    [SerializeField] private GraftedSounds bossSounds;
    //EventInstance introSound;

    Attacks currentAttack = Attacks.NONE;
    Attacks lastAttack = Attacks.NONE;
    AttackState attackState = AttackState.IDLE;
    float attackCooldown = 0;
    bool hasProjectile = true;
    Hero player = null;
    bool playerHit = false;
    float height;

    [SerializeField] float thrustCooldown = 0.5f;
    float thrustCDTimer;
    [SerializeField] float thrustCharge = 1f;
    float thrustChargeTimer;
    [Header("Thrust")]
    [SerializeField] float thrustDuration = 1f;
    float thrustDurationTimer;
    int thrustCounter = 0;

    [SerializeField] float AOEDuration;
    [SerializeField] Transform dashPivot;
    [SerializeField] float dashSpeed = 5f;
    float dashTimer = 0f;
    [Header("Dash")]
    [SerializeField, MinMaxSlider(0, 100)] Vector2 dashRange;
    float AOETimer = 0f;
    bool dashRetracting = false;
    Vector3 originalPos;
    bool triggerAOE = false;

    [Header("Range")]
    [SerializeField] GameObject projectilePrefab;
    GraftedProjectile projectile;

    [Header("Boss Attack Hitboxes")]
    [SerializeField] List<NestedList<Collider>> attacks;

    [SerializeField, Range(0f, 360f)] float visionAngle = 360f;
    [SerializeField] float rotationSpeed = 5f;

    void OnEnable()
    {
        // jouer l'anim de d�but de combat

        // mettre la cam entre le joueur et le boss

        //introSound = AudioManager.Instance.PlaySound(bossSounds.introSound, transform.position);
        //AudioManager.Instance.PlaySound(bossSounds.music);
    }

    private void OnDestroy()
    {
        // stop la musique

        // remettre la camera au dessus du joueur

        if (projectile) Destroy(projectile.gameObject);
    }

    protected override void Start()
    {
        base.Start();
        height = GetComponentInChildren<Renderer>().bounds.size.y;

        animator = GetComponentInChildren<Animator>();

        // hashing animation
        dyingHash = Animator.StringToHash("Dying");
        thrustHash = Animator.StringToHash("Thrust");
        dashHash = Animator.StringToHash("Dash");
        throwingHash = Animator.StringToHash("Throwing");
        retrievingHash = Animator.StringToHash("Retrieving");
    }

    protected override IEnumerator Brain()
    {
        while (true)
        {
            yield return null;

            if (!player)
            {
                player = FindObjectOfType<Hero>();
            }
            else
            {
                Debug.Log(animator.GetCurrentAnimatorStateInfo(0));

                // Face player
                if (attackState != AttackState.ATTACKING)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(player.transform.position - transform.position);
                    lookRotation.x = 0;
                    lookRotation.z = 0;

                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
                }

                // Move towards player
                MoveTo(attackState == AttackState.IDLE ? player.transform.position - (player.transform.position - transform.position).normalized * 2f : transform.position);
                if (agent.destination != transform.position)
                {
                    // son marche
                }

                // Attacks
                if (attackCooldown > 0)
                {
                    attackState = AttackState.IDLE;
                    attackCooldown -= Time.deltaTime;
                    if (attackCooldown < 0) attackCooldown = 0;
                }
                else if (attackCooldown == 0 && currentAttack == Attacks.NONE)
                {
                    lastAttack = currentAttack;
                    currentAttack = ChooseAttack();

                    switch (currentAttack)
                    {
                        case Attacks.RANGE:
                            animator.SetBool(hasProjectile ? throwingHash : retrievingHash, true);
                            break;

                        case Attacks.THRUST:
                            animator.SetBool(thrustHash, true);
                            break;

                        case Attacks.DASH:
                            animator.SetBool(dashHash, true);
                            break;
                    }
                }

                //// DEBUG (commenter tt ce qui est sous "// Attacks" et d�commenter �a)
                //if (Input.GetKeyDown(KeyCode.Alpha1)) currentAttack = Attacks.THRUST;
                //else if (Input.GetKeyDown(KeyCode.Alpha2)) currentAttack = Attacks.DASH;
                //else if (Input.GetKeyDown(KeyCode.Alpha3)) currentAttack = Attacks.RANGE;

                switch (currentAttack)
                {
                    case Attacks.RANGE:
                        if (hasProjectile) ThrowProjectile(); else RetrieveProjectile();
                        break;

                    case Attacks.THRUST:
                        TripleThrust();
                        break;

                    case Attacks.DASH:
                        Dash();
                        break;
                }
            }
        }
    }

    public void Attack(IDamageable _damageable)
    {
        int damages = (int)stats.GetValue(Stat.ATK);

        onHit?.Invoke(_damageable);
        _damageable.ApplyDamage(damages);
    }

    public void ApplyDamage(int _value, bool isCrit = false, bool hasAnimation = true)
    {
        // Some times, this method is call when entity is dead ??
        if (stats.GetValue(Stat.HP) <= 0)
            return;

        if (Vector3.Dot(player.transform.position - transform.position, transform.forward) < 0 && !hasProjectile)
        {
            _value *= 2;
        }

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
            //AudioManager.Instance.PlaySound(bossSounds.deathSound, transform.position);
        }
        else
        {
            //AudioManager.Instance.PlaySound(bossSounds.hitSound, transform.position);
        }
    }

    public void Death()
    {
        Destroy(gameObject);
        //AudioManager.Instance.PlaySound(bossSounds.deathSound);
        GameObject.FindWithTag("Player").GetComponent<Hero>().OnKill?.Invoke(this);
    }

    public void MoveTo(Vector3 _pos)
    {
        agent.SetDestination(_pos);
    }

    public void AttackCollide(List<Collider> colliders, bool _kb = false, bool debugMode = true)
    {
        if (debugMode)
        {
            foreach (Collider collider in colliders)
            {
                collider.gameObject.SetActive(true);
            }
        }

        Vector3 rayOffset = Vector3.up / 2;

        foreach (Collider attackCollider in colliders)
        {
            Collider[] tab = PhysicsExtensions.CheckAttackCollideRayCheck(attackCollider, transform.position + rayOffset, "Player", LayerMask.GetMask("Map"));
            if (tab.Length > 0)
            {
                foreach (Collider col in tab)
                {
                    if (col.gameObject.GetComponent<Hero>() != null)
                    {
                        IDamageable damageable = col.gameObject.GetComponent<IDamageable>();
                        Attack(damageable);

                        if (_kb)
                        {
                            Vector3 knockbackDirection = new Vector3(-transform.forward.z, 0, transform.forward.x);

                            if (Vector3.Cross(transform.forward, player.transform.position - transform.position).y > 0)
                            {
                                knockbackDirection = -knockbackDirection;
                            }

                            ApplyKnockback(damageable, knockbackDirection.normalized);
                        }

                        playerHit = true;
                        break;
                    }
                }
            }
        }
    }

    void DisableHitboxes()
    {
        foreach (NestedList<Collider> attackColliders in attacks)
        {
            foreach (Collider attackCollider in attackColliders.data)
            {
                attackCollider.gameObject.SetActive(false);
            }
        }
    }

    #region Attacks
    void ThrowProjectile()
    {
        projectile = Instantiate(projectilePrefab, transform.position + new Vector3(0, height / 4f, 0), Quaternion.identity).GetComponent<GraftedProjectile>();
        projectile.Initialize(this);
        projectile.SetDirection(player.transform.position - transform.position);

        hasProjectile = false;
        currentAttack = Attacks.NONE;
        attackState = AttackState.IDLE;
        SetAtkCooldown(2f, 0.5f);
        playerHit = false;

        animator.SetBool(throwingHash, false);
    }

    void RetrieveProjectile()
    {
        if (projectile.onTarget)
        {
            projectile.SetDirection(transform.position + new Vector3(0, height / 4f, 0) - projectile.transform.position);
            projectile.SetCollisionImmune(true);
            projectile.onTarget = false;
        }
        else if (!projectile.OnLauncher(transform.position + new Vector3(0, height / 4f, 0)))
        {
            MoveTo(transform.position);
        }
        else
        {
            Destroy(projectile.gameObject);
            hasProjectile = true;
            currentAttack = Attacks.NONE;
            attackState = AttackState.IDLE;
            SetAtkCooldown(2f, 0.5f);
            playerHit = false;
            animator.SetBool(retrievingHash, false);
        }
    }

    void TripleThrust()
    {
        switch (attackState)
        {
            case AttackState.IDLE:

                attackState = AttackState.CHARGING;
                break;

            case AttackState.CHARGING:

                if (thrustChargeTimer < thrustCharge)
                {
                    thrustChargeTimer += Time.deltaTime;
                }
                else
                {
                    AttackCollide(attacks[(int)Attacks.THRUST].data);
                    thrustChargeTimer = 0;
                    attackState = AttackState.ATTACKING;
                }
                break;

            case AttackState.ATTACKING:

                if (thrustDurationTimer < thrustDuration)
                {
                    thrustDurationTimer += Time.deltaTime;
                }
                else
                {
                    DisableHitboxes();
                    thrustDurationTimer = 0;
                    attackState = AttackState.RECOVERING;
                }
                break;

            case AttackState.RECOVERING:

                thrustCounter++;

                if (thrustCounter < 3)
                {
                    attackState = AttackState.IDLE;
                }
                else if (thrustCDTimer < thrustCooldown)
                {
                    thrustCDTimer += Time.deltaTime;
                }
                else
                {
                    thrustCDTimer = 0;
                    thrustCounter = 0;
                    currentAttack = Attacks.NONE;
                    attackState = AttackState.IDLE;
                    SetAtkCooldown(2f, 0.5f);
                    playerHit = false;
                    animator.SetBool(thrustHash, false);
                }
                break;
        }
    }

    void Dash()
    {
        //dashRange.x : min
        //dashRange.y : max

        attackState = AttackState.ATTACKING;

        dashTimer += Time.deltaTime * dashSpeed;

        if (!triggerAOE && !playerHit)
        {
            stats.DecreaseCoeffValue(Stat.ATK, 0.5f);
            AttackCollide(attacks[(int)Attacks.DASH].data, true);
            stats.IncreaseCoeffValue(Stat.ATK, 0.5f);
        }

        if (!dashRetracting)
        {
            if (dashRange.x + dashTimer < dashRange.y)
            {
                originalPos = dashPivot.localPosition;
                dashPivot.localScale = new Vector3(1, 1, dashRange.x + dashTimer);
            }
            else
            {
                dashTimer = 0;
                dashRetracting = true;
            }
        }
        else
        {
            if (dashRange.y - dashTimer > dashRange.x)
            {
                dashPivot.localScale = new Vector3(1, 1, dashRange.y - dashTimer);
                dashPivot.localPosition = originalPos + new Vector3(0, 0, dashTimer);
            }
            else if (!triggerAOE)
            {
                DisableHitboxes();

                transform.position += transform.forward * dashRange.y;
                AttackCollide(attacks[(int)Attacks.DASH + 1].data);
                triggerAOE = true;
            }
            else
            {
                AOETimer += Time.deltaTime;
                if (AOETimer >= AOEDuration)
                {
                    currentAttack = Attacks.NONE;
                    attackState = AttackState.IDLE;
                    playerHit = false;

                    dashRetracting = false;
                    dashTimer = 0;
                    triggerAOE = false;
                    AOETimer = 0;
                    dashPivot.localPosition = originalPos;

                    SetAtkCooldown(0.5f, 0.2f);
                    DisableHitboxes();

                    animator.SetBool(dashHash, false);
                }
            }
        }
    }
    #endregion

    Attacks ChooseAttack()
    {
        float[] attacksProba = new float[3];
        Attacks[] availableAttacks = new Attacks[3];

        availableAttacks[0] = Attacks.THRUST;
        availableAttacks[1] = Attacks.DASH;
        availableAttacks[2] = Attacks.RANGE;

        for (int i = 0; i < attacksProba.Length; i++)
        {
            attacksProba[i] = 100f / attacksProba.Length;

            // �vite que le boss enchaine trop de fois la m�me attaque
            if (lastAttack != Attacks.NONE)
            {
                if (availableAttacks[i] == lastAttack)
                {
                    attacksProba[i] -= 20f;
                }
                else
                {
                    attacksProba[i] += 20f / (attacksProba.Length - 1);
                }
            }
        }

        if (Vector3.Distance(transform.position, player.transform.position) <= stats.GetValue(Stat.ATK_RANGE)) // proche
        {
            attacksProba[0] += 10f;
            attacksProba[2] -= 10f;
        }
        else // loin
        {
            attacksProba[0] -= 10f;
            attacksProba[2] += 10f;
        }

        float randomValue = Random.Range(0, 100f);
        float probaCounter = 0;

        for (int i = 0; i < attacksProba.Length; i++)
        {
            if (attacksProba[i] < 0) attacksProba[i] = 0;

            probaCounter += attacksProba[i];
            if (probaCounter >= randomValue)
            {
                return availableAttacks[i];
            }
        }

        return Attacks.NONE;
    }

    void SetAtkCooldown(float _time, float _randomMargin)
    {
        attackCooldown = _time;
        attackCooldown += Random.Range(-_randomMargin, _randomMargin);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!Selection.Contains(gameObject))
            return;

        DisplayVisionRange(visionAngle);
        DisplayAttackRange(visionAngle);
        DisplayInfos();
    }
#endif
}