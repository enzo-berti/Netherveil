using System;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Entity, IDamageable, IAttacker, IBlastable
{
    public enum PlayerState : int
    {
        DASH = EntityState.NB,
        KNOCKBACK
    }
    Animator animator;
    PlayerInput playerInput;
    PlayerController playerController;
    public Inventory Inventory { get; private set; } = new Inventory();

    public delegate void KillDelegate(IDamageable damageable);
    private KillDelegate onKill;

    public delegate void ChangeRoomDelegate();
    private ChangeRoomDelegate onChangeRoom;

    private IAttacker.AttackDelegate onAttack;
    private IAttacker.HitDelegate onHit;
    public static event Action OnTakeDamage;

    public IAttacker.AttackDelegate OnAttack { get => onAttack; set => onAttack = value; }
    public IAttacker.HitDelegate OnHit { get => onHit; set => onHit = value; }
    public KillDelegate OnKill { get => onKill; set => onKill = value; }
    public ChangeRoomDelegate OnChangeRoom { get => onChangeRoom; set => onChangeRoom = value; }

    public List<Status> StatusToApply => statusToApply;

    protected override void Start()
    {
        base.Start();
        animator = GetComponentInChildren<Animator>();
        playerInput = GetComponent<PlayerInput>();
        playerController = GetComponent<PlayerController>();
        GetComponent<Knockback>().onObstacleCollide += ApplyDamage;

        if (this is IAttacker attacker)
        {
            attacker.OnHit += attacker.ApplyStatus;
        }
    }


    public void ApplyDamage(int _value, bool isCrit = false, bool notEffectDamages = true)
    {
        Stats.DecreaseValue(Stat.HP, _value, false);

        if ((-_value) < 0 && stats.GetValue(Stat.HP) > 0) //just to be sure it really inflicts damages
        {
            if (notEffectDamages)
            {
                DeviceManager.Instance.ForceStopVibrations();
                playerController.ResetValues();
                animator.ResetTrigger("ChargedAttackRelease");
                animator.SetBool("ChargedAttackCasting", false);
                animator.ResetTrigger("BasicAttack");
                AudioManager.Instance.PlaySound(playerController.HitSFX);
                FloatingTextGenerator.CreateEffectDamageText(_value, transform.position, Color.red);
                playerController.HitVFX.Play();
            }

            OnTakeDamage?.Invoke();
        }

        if (stats.GetValue(Stat.HP) <= 0 && State != (int)EntityState.DEAD)
        {
            Death();
            AudioManager.Instance.PlaySound(playerController.DeadSFX);
        }
    }
    public void Death()
    {
        OnDeath?.Invoke(this.transform.position);
        GetComponent<Knockback>().StopAllCoroutines();
        Destroy(GetComponent<CharacterController>());
        playerController.OverridePlayerRotation(210f, true);
        animator.applyRootMotion = true;
        State = (int)EntityState.DEAD;
        animator.ResetTrigger("Death");
        animator.SetTrigger("Death");
    }

    public void Attack(IDamageable damageable)
    {
        int damages = (int)stats.GetValueWithoutCoeff(Stat.ATK);
        if (playerInput.LaunchedChargedAttack)
        {
            damages += (int)(PlayerController.CHARGED_ATTACK_DAMAGES * playerInput.ChargedAttackCoef);
            ApplyKnockback(damageable, stats.GetValue(Stat.KNOCKBACK_DISTANCE) * PlayerController.CHARGED_ATTACK_KNOCKBACK_COEFF * playerInput.ChargedAttackCoef, 
                stats.GetValue(Stat.KNOCKBACK_COEFF) * PlayerController.CHARGED_ATTACK_KNOCKBACK_COEFF * playerInput.ChargedAttackCoef);
        }
        else if (playerController.ComboCount == PlayerController.MAX_COMBO_COUNT - 1)
        {
            damages += PlayerController.FINISHER_DAMAGES;
            DeviceManager.Instance.ApplyVibrations(0.1f, 0f, 0.1f);
            ApplyKnockback(damageable);
        }
        else
        {
            DeviceManager.Instance.ApplyVibrations(0f, 0.1f, 0.1f);
        }

        //bool isCrit = UnityEngine.Random.Range(0, 101) <= stats.GetValue(Stat.CRIT_RATE);
        //if (isCrit)
        //{
        //    float critDamageCoef = stats.GetValue(Stat.CRIT_DAMAGE)/100;
        //    damages = (int)(damages * critDamageCoef);
        //}

        damages = (int)(damages * stats.GetCoeff(Stat.ATK)); 
        damageable.ApplyDamage(damages/*, isCrit*/);

        onHit?.Invoke(damageable);
    }
}