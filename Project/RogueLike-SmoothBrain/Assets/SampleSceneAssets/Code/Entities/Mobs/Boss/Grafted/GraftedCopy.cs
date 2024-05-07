//using FMOD.Studio;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;
//using UnityEngine.VFX;
//using static GlorbStateMachine;

//public class GraftedCopy : Mobs, IGrafted
//{

//    protected override void OnEnable()
//    {
//        base.OnEnable();
//        // jouer l'anim de début de combat

//        // mettre la cam entre le joueur et le boss

//        //StartCoroutine(Brain());
//        if (gameMusic != null)
//        {
//            gameMusic.SetActive(false);
//        }
//        bossSounds.introSound.Play(transform.position);
//        bossSounds.music.Play();
//    }

//    protected override void OnDisable()
//    {
//        base.OnDisable();
//        if (gameMusic != null)
//        {
//            gameMusic.SetActive(true);
//        }
//        bossSounds.introSound.Stop();
//        bossSounds.music.Stop();

//        bossSounds.StopAllSounds();

//        StopAllCoroutines();
//    }

//    private void OnDestroy()
//    {
//        // remettre la camera au dessus du joueur
//        if (gameMusic != null)
//        {
//            gameMusic.SetActive(true);
//        }

//        if (projectile) Destroy(projectile.gameObject);

//        StopAllCoroutines();
//    }

//    protected override void Awake()
//    {
//        base.Awake();
//        gameMusic = GameObject.FindGameObjectWithTag("GameMusic");
//        cameraUtilities = Camera.main.GetComponent<CameraUtilities>();
//    }

//    protected override void Start()
//    {
//        base.Start();

//        height = GetComponentInChildren<Renderer>().bounds.size.y;

//        // hashing animation
//        dyingHash = Animator.StringToHash("Dying");
//        thrustHash = Animator.StringToHash("Thrust");
//        dashHash = Animator.StringToHash("Dash");
//        throwingHash = Animator.StringToHash("Throwing");
//        retrievingHash = Animator.StringToHash("Retrieving");
//        fallHash = Animator.StringToHash("Fall");
//        tripleThrustVFX.transform.parent = null;
//        tripleThrustVFX.Play();
//        player = FindObjectOfType<Hero>();
//    }

//    protected override IEnumerator Brain()
//    {
//        while (true)
//        {
//            //if (stats.GetValue(Stat.HP) > 0)
//            //    animator.speed = isFreeze ? 0 : 1;
//            //else
//            //    animator.speed = 1;

//            yield return new WaitUntil(() => !isFreeze && !IsSpawning);

//            if (deathTimer <= 0)
//            {
//                // Face player
//                if (attackState != AttackState.ATTACKING)
//                {
//                    Quaternion lookRotation = Quaternion.LookRotation(player.transform.position - transform.position);
//                    lookRotation.x = 0;
//                    lookRotation.z = 0;

//                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
//                }

//                // Move towards player
//                MoveTo(attackState == AttackState.IDLE ? player.transform.position - (player.transform.position - transform.position).normalized * 2f : transform.position);
//                if (attackState == AttackState.IDLE)
//                {
//                    bossSounds.walkingSound.Play(transform.position);
//                }

//                // Attacks
//                if (attackCooldown > 0)
//                {
//                    attackState = AttackState.IDLE;
//                    attackCooldown -= Time.deltaTime;
//                    if (attackCooldown < 0) attackCooldown = 0;
//                }
//                else if (attackCooldown == 0 && currentAttack == Attacks.NONE)
//                {
//                    lastAttack = currentAttack;
//                    currentAttack = ChooseAttack();

//                    switch (currentAttack)
//                    {
//                        case Attacks.RANGE:
//                            animator.SetBool(hasProjectile ? throwingHash : retrievingHash, true);
//                            break;

//                        case Attacks.THRUST:
//                            animator.SetBool(thrustHash, true);
//                            break;

//                        case Attacks.DASH:
//                            animator.SetBool(dashHash, true);
//                            break;
//                    }
//                }

//                // DEBUG (commenter tt ce qui est sous "// Attacks" et décommenter ça)
//                //if (Input.GetKeyDown(KeyCode.Alpha1))
//                //{
//                //    currentAttack = Attacks.THRUST;
//                //    animator.SetBool(thrustHash, true);
//                //}
//                //else if (Input.GetKeyDown(KeyCode.Alpha2))
//                //{
//                //    currentAttack = Attacks.DASH;
//                //    animator.SetBool(dashHash, true);
//                //}
//                //else if (Input.GetKeyDown(KeyCode.Alpha3))
//                //{
//                //    currentAttack = Attacks.RANGE;
//                //    animator.SetBool(hasProjectile ? throwingHash : retrievingHash, true);
//                //}

//                switch (currentAttack)
//                {
//                    case Attacks.RANGE:
//                        if (hasProjectile) ThrowProjectile(); else RetrieveProjectile();
//                        break;

//                    case Attacks.THRUST:
//                        TripleThrust();
//                        break;

//                    case Attacks.DASH:
//                        Dash();
//                        break;
//                }
//            }
//            else
//            {
//                deathTimer -= Time.deltaTime;
//                if (deathTimer <= 0)
//                {
//                    deathTimer = Time.deltaTime;
//                    if (bossSounds.deathSound.GetState() != PLAYBACK_STATE.PLAYING)
//                    {
//                        Utilities.Hero.OnKill?.Invoke(this);
//                        OnDeath?.Invoke(transform.position);
//                        bossSounds.StopAllSounds();
//                        Destroy(transform.parent.gameObject);
//                    }
//                }
//            }

//        }
//    }

//    public void Attack(IDamageable _damageable, int additionalDamages = 0)
//    {
//        int damages = (int)stats.GetValue(Stat.ATK);
//        damages += additionalDamages;

//        onHit?.Invoke(_damageable, this);
//        _damageable.ApplyDamage(damages, this);
//    }

//    public void ApplyDamage(int _value, IAttacker attacker, bool notEffectDamage = true)
//    {
//        if ((Vector3.Dot(player.transform.position - transform.position, transform.forward) < 0 && !hasProjectile)
//            || currentAttack == Attacks.RANGE)
//        {
//            _value *= 2;
//        }

//        ApplyDamagesMob(_value, bossSounds.hitSound, Death, notEffectDamage);
//    }

//    public void Death()
//    {
//        animator.speed = 1;
//        OnDeath?.Invoke(transform.position);
//        Utilities.Hero.OnKill?.Invoke(this);
//        deathTimer = 0.5f;
//        MoveTo(transform.position);
//        animator.SetBool(dyingHash, true);
//        bossSounds.deathSound.Play(transform.position);
//    }

//    public void MoveTo(Vector3 _pos)
//    {
//        agent.SetDestination(_pos);
//    }

//    public void AttackCollide(List<Collider> colliders, bool _kb = false, bool debugMode = true)
//    {
//        if (debugMode)
//        {
//            foreach (Collider collider in colliders)
//            {
//                collider.gameObject.SetActive(true);
//            }
//        }

//        Vector3 rayOffset = Vector3.up / 2;

//        foreach (Collider attackCollider in colliders)
//        {
//            Collider[] tab = PhysicsExtensions.CheckAttackCollideRayCheck(attackCollider, transform.position + rayOffset, "Player", LayerMask.GetMask("Map"));
//            if (tab.Length > 0)
//            {
//                foreach (Collider col in tab)
//                {
//                    if (col.gameObject.GetComponent<Hero>() != null)
//                    {
//                        IDamageable damageable = col.gameObject.GetComponent<IDamageable>();
//                        Attack(damageable);

//                        if (_kb)
//                        {
//                            //Vector3 knockbackDirection = new Vector3(-transform.forward.z, 0, transform.forward.x);

//                            //if (Vector3.Cross(transform.forward, player.transform.position - transform.position).y > 0)
//                            //{
//                            //    knockbackDirection = -knockbackDirection;
//                            //}

//                            ApplyKnockback(damageable, this);
//                        }

//                        playerHit = true;
//                        break;
//                    }
//                }
//            }
//        }
//    }

//    void DisableHitboxes()
//    {
//        foreach (NestedList<Collider> attackColliders in attacks)
//        {
//            foreach (Collider attackCollider in attackColliders.data)
//            {
//                attackCollider.gameObject.SetActive(false);
//            }
//        }
//    }

//    #region Attacks
//    void ThrowProjectile()
//    {
//        attackState = AttackState.ATTACKING;

//        if (throwingTimer == 0)
//        {
//            bossSounds.weaponOutSound.Play(transform.position);
//        }

//        throwingTimer += Time.deltaTime;

//        if (throwingTimer > 0.7f)
//        {
//            projectile = Instantiate(projectilePrefab, transform.position + new Vector3(0, height / 6f, 0), Quaternion.identity).GetComponent<GraftedProjectile>();
//            projectile.Initialize(this);

//            Vector3 direction = player.transform.position - transform.position;
//            direction.y = 0;
//            projectile.SetDirection(direction);

//            hasProjectile = false;
//            currentAttack = Attacks.NONE;
//            attackState = AttackState.IDLE;
//            SetAtkCooldown(2f, 0.5f);
//            playerHit = false;

//            throwingTimer = 0;

//            bossSounds.projectileLaunchedSound.Play(transform.position);

//            animator.SetBool(throwingHash, false);
//        }
//    }

//    void RetrieveProjectile()
//    {
//        if (!projectile.onTarget && !projectile.GetCollisionImmune())
//        {
//            currentAttack = Attacks.NONE;
//            attackCooldown = 0;
//            return;
//        }

//        rangeSecurity += Time.deltaTime;

//        attackState = AttackState.ATTACKING;

//        projectile.SetTempSpeed(projectile.Speed * 0.25f);

//        bossSounds.retrievingProjectileSound.Play(transform.position);

//        if (projectile.onTarget)
//        {
//            projectile.SetDirection(transform.position + new Vector3(0, height / 6f, 0) - projectile.transform.position);
//            projectile.SetCollisionImmune(true);
//            projectile.onTarget = false;
//        }
//        else if (!projectile.OnLauncher(transform.position + new Vector3(0, height / 6f, 0)) && rangeSecurity <= 5f)
//        {
//            MoveTo(transform.position);
//        }
//        else
//        {
//            Destroy(projectile.gameObject);
//            hasProjectile = true;
//            currentAttack = Attacks.NONE;
//            attackState = AttackState.IDLE;
//            SetAtkCooldown(2f, 0.5f);
//            playerHit = false;
//            animator.SetBool(retrievingHash, false);
//            bossSounds.retrievingProjectileSound.Stop();
//            rangeSecurity = 0f;
//        }
//    }

//    void TripleThrust()
//    {
//        switch (attackState)
//        {
//            case AttackState.IDLE:

//                attackState = AttackState.CHARGING;
//                break;

//            case AttackState.CHARGING:

//                if (thrustChargeTimer < (thrustCounter == 0 ? thrustCharge : 0.5f))
//                {
//                    thrustChargeTimer += Time.deltaTime;
//                }
//                else
//                {
//                    AttackCollide(attacks[(int)Attacks.THRUST].data, debugMode: false);
//                    thrustChargeTimer = 0;
//                    attackState = AttackState.ATTACKING;

//                    bossSounds.thrustSound.Play(transform.position, true);

//                    DeviceManager.Instance.ApplyVibrations(0.8f, 0.8f, 0.25f);
//                    cameraUtilities.ShakeCamera(0.3f, 0.25f, EasingFunctions.EaseInQuint);
//                }
//                break;

//            case AttackState.ATTACKING:

//                if (thrustDurationTimer < thrustDuration)
//                {
//                    if (tripleThrustCoroutine == null && thrustDurationTimer == 0f)
//                    {
//                        tripleThrustCoroutine = StartCoroutine(TripleThrustVFX());
//                    }
//                    else if (tripleThrustCoroutine != null && thrustDurationTimer == 0f)
//                    {
//                        StopCoroutine(tripleThrustCoroutine);
//                        tripleThrustCoroutine = StartCoroutine(TripleThrustVFX());
//                    }

//                    thrustDurationTimer += Time.deltaTime;
//                }
//                else
//                {
//                    DisableHitboxes();
//                    thrustDurationTimer = 0;
//                    attackState = AttackState.RECOVERING;
//                }
//                break;

//            case AttackState.RECOVERING:

//                thrustCounter++;

//                if (thrustCounter < 3)
//                {
//                    //if (tripleThrustCoroutine != null)
//                    //{
//                    //    StopCoroutine(tripleThrustCoroutine);
//                    //}
//                    //tripleThrustCoroutine = null;
//                    attackState = AttackState.CHARGING;
//                }
//                else
//                {
//                    thrustCounter = 0;
//                    currentAttack = Attacks.NONE;
//                    attackState = AttackState.IDLE;
//                    SetAtkCooldown(2f, 0.5f);
//                    playerHit = false;
//                    animator.SetBool(thrustHash, false);
//                }
//                break;
//        }
//    }

//    private IEnumerator TripleThrustVFX()
//    {
//        //tripleThrustVFX.Play();
//        tripleThrustVFX.transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
//        Vector3 endPos = tripleThrustVFX.transform.position + transform.forward * attacks[(int)Attacks.THRUST].data[0].transform.localScale.z;
//        while (tripleThrustVFX.transform.position != endPos)
//        {
//            tripleThrustVFX.transform.position = Vector3.MoveTowards(tripleThrustVFX.transform.position, endPos, 23f * Time.deltaTime);
//            yield return null;
//        }

//        //tripleThrustVFX.Stop();
//    }

//    void Dash()
//    {
//        attackState = AttackState.ATTACKING;

//        dashChargeTimer += Time.deltaTime;

//        if (dashChargeTimer <= 0.3f)
//        {
//            attackState = AttackState.CHARGING;
//        }
//        else
//        {
//            animator.SetBool(fallHash, true);

//            if (!dashVFXPlayed && dashChargeTimer >= 0.5f)
//            {
//                bossSounds.stretchSound.Play(transform.position);
//                dashVFX.GetComponent<VFXStopper>().PlayVFX();
//                dashVFXPlayed = true;

//                DeviceManager.Instance.ApplyVibrations(0.8f, 0.8f, 0.3f);
//                cameraUtilities.ShakeCamera(0.5f, 0.3f, EasingFunctions.EaseInQuint);
//            }
//        }

//        if (attackState == AttackState.ATTACKING)
//        {
//            travelledDistance += Time.deltaTime * dashSpeed;

//            if (!triggerAOE && !playerHit)
//            {
//                AttackCollide(attacks[(int)Attacks.DASH].data, debugMode: false);
//            }

//            if (travelledDistance <= dashRange)
//            {
//                agent.Warp(transform.position + transform.forward * Time.deltaTime * dashSpeed);
//            }
//            else if (!triggerAOE)
//            {
//                DisableHitboxes();

//                bossSounds.spinAttackSound.Play(transform.position);

//                playerHit = false;
//                triggerAOE = true;
//            }
//            else
//            {
//                if (!playerHit)
//                {
//                    AttackCollide(attacks[(int)Attacks.DASH + 1].data, debugMode: false);
//                }

//                AOETimer += Time.deltaTime;

//                agent.Warp(transform.position + transform.forward * Time.deltaTime * dashSpeed * 0.15f);
//                if (AOETimer >= AOEDuration)
//                {
//                    currentAttack = Attacks.NONE;
//                    attackState = AttackState.IDLE;
//                    playerHit = false;

//                    travelledDistance = 0;
//                    dashChargeTimer = 0;
//                    triggerAOE = false;
//                    AOETimer = 0;

//                    SetAtkCooldown(0.5f, 0.2f);
//                    DisableHitboxes();

//                    dashVFXPlayed = false;

//                    animator.SetBool(dashHash, false);
//                    animator.SetBool(fallHash, false);
//                }
//            }
//        }
//    }
//    #endregion

//    Attacks ChooseAttack()
//    {
//        float[] attacksProba = new float[3];
//        Attacks[] availableAttacks = new Attacks[3];

//        availableAttacks[0] = Attacks.THRUST;
//        availableAttacks[1] = Attacks.DASH;
//        availableAttacks[2] = Attacks.RANGE;

//        for (int i = 0; i < attacksProba.Length; i++)
//        {
//            attacksProba[i] = 100f / attacksProba.Length;

//            // évite que le boss enchaine trop de fois la même attaque
//            if (lastAttack != Attacks.NONE)
//            {
//                if (availableAttacks[i] == lastAttack)
//                {
//                    attacksProba[i] -= 20f;
//                }
//                else
//                {
//                    attacksProba[i] += 20f / (attacksProba.Length - 1);
//                }
//            }
//        }

//        if (Vector3.Distance(transform.position, player.transform.position) <= stats.GetValue(Stat.ATK_RANGE)) // proche
//        {
//            attacksProba[0] += 10f;
//            attacksProba[2] -= 10f;
//        }
//        else // loin
//        {
//            attacksProba[0] -= 10f;
//            attacksProba[2] += 10f;
//        }

//        float randomValue = Random.Range(0, 100f);
//        float probaCounter = 0;

//        for (int i = 0; i < attacksProba.Length; i++)
//        {
//            if (attacksProba[i] < 0) attacksProba[i] = 0;

//            probaCounter += attacksProba[i];
//            if (probaCounter >= randomValue)
//            {
//                return availableAttacks[i];
//            }
//        }

//        return Attacks.NONE;
//    }

//    void SetAtkCooldown(float _time, float _randomMargin)
//    {
//        attackCooldown = _time;
//        attackCooldown += Random.Range(-_randomMargin, _randomMargin);
//    }

//#if UNITY_EDITOR
//    private void OnDrawGizmos()
//    {
//        if (!Selection.Contains(gameObject))
//            return;

//        DisplayVisionRange(visionAngle);
//        DisplayAttackRange(visionAngle);
//        DisplayInfos();
//    }
//#endif
//}