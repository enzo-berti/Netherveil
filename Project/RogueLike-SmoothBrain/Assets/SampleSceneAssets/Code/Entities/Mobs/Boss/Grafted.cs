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
    float deathTimer = 0;

    int dyingHash;
    int thrustHash;
    int dashHash;
    int throwingHash;
    int retrievingHash;
    int fallHash;

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
        public AudioManager.Sound deathSound = new("Death"); //
        public AudioManager.Sound hitSound = new("Hit"); //
        public AudioManager.Sound projectileLaunchedSound = new("Projectile launched"); //
        public AudioManager.Sound projectileHitMapSound = new("Projectile hit map");
        public AudioManager.Sound projectileHitPlayerSound = new("Projectile hit player");
        public AudioManager.Sound thrustSound = new("Thrust"); //
        public AudioManager.Sound introSound = new("Intro");
        public AudioManager.Sound retrievingProjectileSound = new("Retrieving projectile"); //
        public AudioManager.Sound spinAttackSound = new("Fall"); //
        public AudioManager.Sound stretchSound = new("Dash"); //
        public AudioManager.Sound weaponOutSound = new("WeaponOut"); //
        public AudioManager.Sound weaponInSound = new("WeaponIn");
        public AudioManager.Sound walkingSound = new("Walk"); //
        public AudioManager.Sound music = new("Music");
    }

    [Header("Sounds")]
    [SerializeField] private GraftedSounds bossSounds;

    Attacks currentAttack = Attacks.NONE;
    Attacks lastAttack = Attacks.NONE;
    AttackState attackState = AttackState.IDLE;
    float attackCooldown = 0;
    bool hasProjectile = true;
    Hero player = null;
    bool playerHit = false;
    float height;

    [SerializeField] float thrustCharge = 1f;
    float thrustChargeTimer;
    [Header("Thrust")]
    [SerializeField] float thrustDuration = 1f;
    float thrustDurationTimer;
    int thrustCounter = 0;

    [SerializeField] float AOEDuration;
    [SerializeField] float dashSpeed = 5f;
    float dashChargeTimer = 0f;
    float travelledDistance = 0f;
    [Header("Dash")]
    [SerializeField] float dashRange;
    float AOETimer = 0f;
    bool triggerAOE = false;

    [Header("Range")]
    [SerializeField] GameObject projectilePrefab;
    GraftedProjectile projectile;
    float throwingTimer = 0f;

    [Header("Boss Attack Hitboxes")]
    [SerializeField] List<NestedList<Collider>> attacks;

    [SerializeField, Range(0f, 360f)] float visionAngle = 360f;
    [SerializeField] float rotationSpeed = 5f;

    void OnEnable()
    {
        // jouer l'anim de début de combat

        // mettre la cam entre le joueur et le boss

        //StartCoroutine(Brain());

        AudioManager.Instance.PlaySound(bossSounds.introSound, transform.position);
        AudioManager.Instance.PlaySound(bossSounds.music);
    }

    private void OnDisable()
    {
        AudioManager.Instance.StopSound(bossSounds.introSound);
        AudioManager.Instance.StopSound(bossSounds.music);

        //StopAllCoroutines();
    }

    private void OnDestroy()
    {
        // remettre la camera au dessus du joueur

        AudioManager.Instance.StopSound(bossSounds.music);

        if (projectile) Destroy(projectile.gameObject);

        StopAllCoroutines();
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
        fallHash = Animator.StringToHash("Fall");

        transform.position = transform.parent.position;
        transform.parent.position = Vector3.zero;
    }

    protected override IEnumerator Brain()
    {
        while (true)
        {
            yield return null;

            if (deathTimer <= 0)
            {
                if (!player)
                {
                    player = FindObjectOfType<Hero>();
                }
                else
                {
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
                    if (attackState == AttackState.IDLE)
                    {
                        AudioManager.Instance.PlaySound(bossSounds.walkingSound, transform.position);
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

                    //// DEBUG (commenter tt ce qui est sous "// Attacks" et décommenter ça)
                    //if (Input.GetKeyDown(KeyCode.Alpha1))
                    //{
                    //    currentAttack = Attacks.THRUST;
                    //    animator.SetBool(thrustHash, true);
                    //}
                    //else if (Input.GetKeyDown(KeyCode.Alpha2))
                    //{
                    //    currentAttack = Attacks.DASH;
                    //    animator.SetBool(dashHash, true);
                    //}
                    //else if (Input.GetKeyDown(KeyCode.Alpha3))
                    //{
                    //    currentAttack = Attacks.RANGE;
                    //    animator.SetBool(hasProjectile ? throwingHash : retrievingHash, true);
                    //}

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
            else
            {
                deathTimer -= Time.deltaTime;
                if (deathTimer <= 0)
                {
                    deathTimer = Time.deltaTime;
                    if (bossSounds.deathSound.GetState() != PLAYBACK_STATE.PLAYING)
                    {
                        Destroy(gameObject);
                        GameObject.FindWithTag("Player").GetComponent<Hero>().OnKill?.Invoke(this);
                        AudioManager.Instance.StopSound(bossSounds.music);
                    }
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

        if ((Vector3.Dot(player.transform.position - transform.position, transform.forward) < 0 && !hasProjectile)
            || currentAttack == Attacks.RANGE)
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
        }
        else
        {
            AudioManager.Instance.PlaySound(bossSounds.hitSound, transform.position, true);
        }
    }

    public void Death()
    {
        deathTimer = 0.5f;
        MoveTo(transform.position);
        animator.SetBool(dyingHash, true);
        AudioManager.Instance.PlaySound(bossSounds.deathSound, transform.position);
        GameObject.FindGameObjectWithTag("WINSCREEN").gameObject.SetActive(true);
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
                            //Vector3 knockbackDirection = new Vector3(-transform.forward.z, 0, transform.forward.x);

                            //if (Vector3.Cross(transform.forward, player.transform.position - transform.position).y > 0)
                            //{
                            //    knockbackDirection = -knockbackDirection;
                            //}

                            ApplyKnockback(damageable);
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
        attackState = AttackState.ATTACKING;

        if (throwingTimer == 0)
        {
            AudioManager.Instance.PlaySound(bossSounds.weaponOutSound, transform.position);
        }

        throwingTimer += Time.deltaTime;

        if (throwingTimer > 0.7f)
        {
            projectile = Instantiate(projectilePrefab, transform.position - new Vector3(0, height / 6f, 0), Quaternion.identity).GetComponent<GraftedProjectile>();
            projectile.Initialize(this);

            Vector3 direction = player.transform.position - transform.position;
            direction.y = 0;
            projectile.SetDirection(direction);

            hasProjectile = false;
            currentAttack = Attacks.NONE;
            attackState = AttackState.IDLE;
            SetAtkCooldown(2f, 0.5f);
            playerHit = false;

            throwingTimer = 0;

            AudioManager.Instance.PlaySound(bossSounds.projectileLaunchedSound, transform.position);

            animator.SetBool(throwingHash, false);
        }
    }

    void RetrieveProjectile()
    {
        attackState = AttackState.ATTACKING;

        projectile.SetTempSpeed(projectile.Speed * 0.25f);

        AudioManager.Instance.PlaySound(bossSounds.retrievingProjectileSound, transform.position);

        if (projectile.onTarget)
        {
            projectile.SetDirection(transform.position - new Vector3(0, height / 6f, 0) - projectile.transform.position);
            projectile.SetCollisionImmune(true);
            projectile.onTarget = false;
        }
        else if (!projectile.OnLauncher(transform.position - new Vector3(0, height / 6f, 0)))
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
            AudioManager.Instance.StopSound(bossSounds.retrievingProjectileSound);
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

                if (thrustChargeTimer < (thrustCounter == 0 ? thrustCharge : 0.5f))
                {
                    thrustChargeTimer += Time.deltaTime;
                }
                else
                {
                    AttackCollide(attacks[(int)Attacks.THRUST].data);
                    thrustChargeTimer = 0;
                    attackState = AttackState.ATTACKING;

                    AudioManager.Instance.PlaySound(bossSounds.thrustSound, transform.position, true);
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
                    attackState = AttackState.CHARGING;
                }
                else
                {
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
        attackState = AttackState.ATTACKING;

        if (dashChargeTimer <= 0.3f)
        {
            dashChargeTimer += Time.deltaTime;
            attackState = AttackState.CHARGING;
        }
        else
        {
            if (!animator.GetBool(fallHash))
            {
                AudioManager.Instance.PlaySound(bossSounds.stretchSound, transform.position);
            }

            animator.SetBool(fallHash, true);
        }

        if (attackState == AttackState.ATTACKING)
        {
            travelledDistance += Time.deltaTime * dashSpeed;

            if (!triggerAOE && !playerHit)
            {
                AttackCollide(attacks[(int)Attacks.DASH].data, true);
            }

            if (travelledDistance <= dashRange)
            {
                agent.Warp(transform.position + transform.forward * Time.deltaTime * dashSpeed);
            }
            else if (!triggerAOE)
            {
                DisableHitboxes();

                AudioManager.Instance.PlaySound(bossSounds.spinAttackSound, transform.position);

                playerHit = false;
                triggerAOE = true;
            }
            else
            {
                if (!playerHit)
                {
                    AttackCollide(attacks[(int)Attacks.DASH + 1].data, true);
                }

                AOETimer += Time.deltaTime;

                agent.Warp(transform.position + transform.forward * Time.deltaTime * dashSpeed * 0.15f);
                if (AOETimer >= AOEDuration)
                {
                    currentAttack = Attacks.NONE;
                    attackState = AttackState.IDLE;
                    playerHit = false;

                    travelledDistance = 0;
                    dashChargeTimer = 0;
                    triggerAOE = false;
                    AOETimer = 0;

                    SetAtkCooldown(0.5f, 0.2f);
                    DisableHitboxes();

                    animator.SetBool(dashHash, false);
                    animator.SetBool(fallHash, false);
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

            // évite que le boss enchaine trop de fois la même attaque
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