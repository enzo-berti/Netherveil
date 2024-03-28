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

    [SerializeField, Range(0f, 360f)] private float visionAngle = 360f;

    [Header("Boss parameters")]
    Hero player = null;
    bool playerHit = false;
    float height;

    [Header("Boss Attack Hitboxes")]
    [SerializeField] List<NestedList<Collider>> attacks;

    [Header("Thrust")]
    [SerializeField] float thrustCooldown = 0.5f;
    float thrustCDTimer;
    [SerializeField] float thrustCharge = 1f;
    float thrustChargeTimer;
    [SerializeField] float thrustDuration = 1f;
    float thrustDurationTimer;
    int thrustCounter = 0;

    [Header("Dash")]
    [SerializeField, MinMaxSlider(0, 100)] Vector2 dashRange;
    [SerializeField] Transform dashPivot;
    [SerializeField] float dashSpeed = 5f;
    float dashTimer = 0f;
    [SerializeField] float AOEDuration;
    float AOETimer = 0f;
    bool dashRetracting = false;
    Vector3 originalPos;
    bool triggerAOE = false;

    [Header("Range")]
    [SerializeField] GameObject projectilePrefab;
    GraftedProjectile projectile;


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

    Attacks currentAttack = Attacks.NONE; // Commenter par Dorian -> WARNING
    AttackState attackState = AttackState.IDLE;
    float attackCooldown = 0; // Commenter par Dorian -> WARNING
    bool hasProjectile = true;

    protected override void Start()
    {
        base.Start();
        height = GetComponent<Renderer>().bounds.size.y;
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
            //else
            //{
            //    // Face player
            //    if (attackState != AttackState.ATTACKING)
            //    {
            //        Quaternion lookRotation = Quaternion.LookRotation(player.transform.position - transform.position);
            //        lookRotation.x = 0;
            //        lookRotation.z = 0;

            //        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 5f * Time.deltaTime);
            //    }

            //    // Move towards player
            //    MoveTo(attackState == AttackState.IDLE ? player.transform.position - (player.transform.position - transform.position).normalized * 2f : transform.position);

            //    // Attacks
            //    if (attackCooldown > 0)
            //    {
            //        attackState = AttackState.IDLE;
            //        attackCooldown -= Time.deltaTime;
            //        if (attackCooldown < 0) attackCooldown = 0;
            //    }
            //    else if (attackCooldown == 0)
            //    {
            //        //currentAttack = (Attacks)Random.Range(0, 3);
            //        currentAttack = Attacks.DASH;
            //    }

            //    switch (currentAttack)
            //    {
            //        case Attacks.RANGE:
            //            if (hasProjectile) ThrowProjectile(); else RetrieveProjectile();
            //            break;

            //        case Attacks.THRUST:
            //            TripleThrust();
            //            break;

            //        case Attacks.DASH:
            //            Dash();
            //            break;
            //    }
            //}
            projectile = Instantiate(projectilePrefab, transform.position + new Vector3(0, height / 2f, 0), Quaternion.identity).GetComponent<GraftedProjectile>();
            projectile.Initialize(player.transform.position - transform.position);
            hasProjectile = false;
        }
    }

    public void Attack(IDamageable _damageable)
    {
        int damages = (int)(stats.GetValue(Stat.ATK) * stats.GetValue(Stat.ATK_COEFF));
        onHit?.Invoke(_damageable);
        _damageable.ApplyDamage(damages);
    }

    public void ApplyDamage(int _value, bool isCrit = false, bool hasAnimation = true)
    {
        Stats.DecreaseValue(Stat.HP, _value, false);

        if (hasAnimation)
        {
            FloatingTextGenerator.CreateDamageText(_value, transform.position, isCrit);
            //add SFX here
        }
        if (stats.GetValue(Stat.HP) <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        Destroy(gameObject);
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
                        //if (_kb)
                        //{
                        //    Knockback knockbackable = (damageable as MonoBehaviour).GetComponent<Knockback>();
                        //    if (knockbackable)
                        //    {
                        //        Vector3 damageablePos = (damageable as MonoBehaviour).transform.position;
                        //        Vector3 force = new Vector3(-(damageablePos.z - transform.position.z), 0.0f, damageablePos.x - transform.position.x).normalized;
                        //        knockbackable.GetKnockback(force, 5.0f, stats.GetValue(Stat.KNOCKBACK_COEFF));
                        //        FloatingTextGenerator.CreateActionText((damageable as MonoBehaviour).transform.position, "Pushed!");
                        //    }
                        //}

                        playerHit = true;
                        break;
                    }
                }
            }
        }
    }

    ///////////////////////////////// ATTACKS

    void ThrowProjectile()
    {
        hasProjectile = false;
        currentAttack = Attacks.NONE;
        attackState = AttackState.IDLE;
    }

    void RetrieveProjectile()
    {
        hasProjectile = true;
        currentAttack = Attacks.NONE;
        attackState = AttackState.IDLE;
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
                    attackCooldown = 2f;
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
            AttackCollide(attacks[(int)Attacks.DASH].data, true);
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

                    attackCooldown = 0.5f;
                    DisableHitboxes();
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

#if UNITY_EDITOR
    //private void OnDrawGizmos()
    //{
    //    if (!Selection.Contains(gameObject))
    //        return;

    //    DisplayVisionRange(visionAngle);
    //    DisplayAttackRange(visionAngle);
    //    DisplayInfos();
    //}
#endif
}