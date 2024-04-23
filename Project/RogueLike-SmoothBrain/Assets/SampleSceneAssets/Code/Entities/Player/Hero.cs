using PostProcessingEffects;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Entity, IDamageable, IAttacker, IBlastable
{
    public enum PlayerState : int
    {
        DASH = EntityState.NB,
        KNOCKBACK,
        UPGRADING_STATS
    }
    Animator animator;
    PlayerInput playerInput;
    PlayerController playerController;
    public Inventory Inventory { get; private set; } = new Inventory();

    private static event Action<IDamageable> onKill;

    private event IAttacker.AttackDelegate onAttack;
    private event IAttacker.HitDelegate onAttackHit;
    public static event Action<int, IAttacker> OnTakeDamage;
    public static event Action<IDamageable, IAttacker> OnBasicAttack;
    public static event Action<IDamageable, IAttacker> OnDashAttack;
    public static event Action<IDamageable, IAttacker> OnSpearAttack;
    public static event Action<IDamageable, IAttacker> OnChargedAttack;
    public static event Action<IDamageable, IAttacker> OnFinisherAttack;
    public static event Action OnQuestObtained;
    public static event Action OnQuestFinished;
    public static event Action<ISpecialAbility> OnBenedictionMaxUpgrade;
    public static event Action<ISpecialAbility> OnCorruptionMaxUpgrade;
    public static event Action OnBenedictionMaxDrawback;
    public static event Action OnCorruptionMaxDrawback;

    public delegate void OnBeforeApplyDamagesDelegate(ref int damages, IDamageable target);
    public static event OnBeforeApplyDamagesDelegate OnBeforeApplyDamages;

    public IAttacker.AttackDelegate OnAttack { get => onAttack; set => onAttack = value; }
    public IAttacker.HitDelegate OnAttackHit { get => onAttackHit; set => onAttackHit = value; }
    public static Action<IDamageable> OnKill { get => onKill; set => onKill = value; }

    int currentStep = 0;
    public readonly int STEP_VALUE = 25;
    public bool CanHealFromConsumables { get; set; } = true;

    public List<Status> StatusToApply => statusToApply;

    Quest currentQuest = null;
    public Quest CurrentQuest
    {
        get => currentQuest;
        set
        {
            currentQuest = value;

            if (value == null)
            {
                OnQuestFinished?.Invoke();
            }
            else
            {
                value.AcceptQuest();
                OnQuestObtained?.Invoke();
            }
        }
    }

    protected override void Start()
    {
        base.Start();
        animator = GetComponentInChildren<Animator>();
        playerInput = GetComponent<PlayerInput>();
        playerController = GetComponent<PlayerController>();
        GetComponent<Knockback>().onObstacleCollide += ApplyDamage;

        if (this is IAttacker attacker)
        {
            attacker.OnAttackHit += attacker.ApplyStatus;
        }

        stats.onStatChange += UpgradePlayerStats;
    }

    private void OnEnable()
    {
        OnTakeDamage += (dam, atk) => PostProcessingEffectManager.current.Play(Effect.Hit, false);
    }

    private void OnDisable()
    {
        OnTakeDamage -= (dam, atk) => PostProcessingEffectManager.current.Play(Effect.Hit, false);
    }

    public void ApplyDamage(int _value, IAttacker attacker, bool notEffectDamages = true)
    {
        if(IsInvincibleCount > 0)
        {
            return;
        }

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

            OnTakeDamage?.Invoke(_value, attacker);
        }

        if (stats.GetValue(Stat.HP) <= 0 && State != (int)EntityState.DEAD)
        {
            Death();
            AudioManager.Instance.PlaySound(playerController.DeadSFX);
        }
    }
    public void Death()
    {
        animator.speed = 1;
        OnDeath?.Invoke(this.transform.position);
        GetComponent<Knockback>().StopAllCoroutines();
        Destroy(GetComponent<CharacterController>());
        playerController.OverridePlayerRotation(210f, true);
        animator.applyRootMotion = true;
        State = (int)EntityState.DEAD;
        animator.ResetTrigger("Death");
        animator.SetTrigger("Death");
    }

    public void Attack(IDamageable damageable, int additionalDamages = 0)
    {
        int damages = (int)stats.GetValueWithoutCoeff(Stat.ATK);

        if (playerInput.LaunchedChargedAttack)
        {
            damages += (int)(PlayerController.CHARGED_ATTACK_DAMAGES * playerInput.ChargedAttackCoef);
            ApplyKnockback(damageable, this, stats.GetValue(Stat.KNOCKBACK_DISTANCE) * PlayerController.CHARGED_ATTACK_KNOCKBACK_COEFF * playerInput.ChargedAttackCoef,
                stats.GetValue(Stat.KNOCKBACK_COEFF) * PlayerController.CHARGED_ATTACK_KNOCKBACK_COEFF * playerInput.ChargedAttackCoef);
            OnChargedAttack?.Invoke(damageable, this);
        }
        else if (playerController.ComboCount == PlayerController.MAX_COMBO_COUNT - 1)
        {
            damages += PlayerController.FINISHER_DAMAGES;
            DeviceManager.Instance.ApplyVibrations(0.1f, 0f, 0.1f);
            ApplyKnockback(damageable, this);
            OnFinisherAttack?.Invoke(damageable, this);
        }
        else if (playerController.Spear.IsThrowing || playerController.Spear.IsThrown)
        {
            damages += PlayerController.SPEAR_DAMAGES;
            OnSpearAttack?.Invoke(damageable, this);
        }
        else if (playerInput.LaunchedDashAttack)
        {
            OnDashAttack?.Invoke(damageable, this);
        }
        else
        {
            OnBasicAttack?.Invoke(damageable, this);
        }

        damages += additionalDamages;

        OnBeforeApplyDamages?.Invoke(ref damages, damageable);
        damages = (int)(damages * stats.GetCoeff(Stat.ATK)); 
        damageable.ApplyDamage(damages, this);

        OnAttackHit?.Invoke(damageable, this);
    }

    #region Corruption&BenedictionManagement
    public void UpgradePlayerStats(Stat stat)
    {
        if (stat != Stat.CORRUPTION)
            return;

        float corruptionStat = Stats.GetValue(stat);
        float corruptionLastValue = Stats.GetLastValue(stat);
        float diff = corruptionStat - corruptionLastValue;

        ManageCorruptionChangeLessThanStep(corruptionStat, corruptionLastValue, diff);
        ManageCorruptionChangeMoreThanStep(corruptionStat, corruptionLastValue, diff);

        //ensure that player doesn't die by stat upgrade
        if (Stats.GetValue(Stat.HP) <= 0f)
        {
            Stats.SetValue(Stat.HP, 1f);
        }
    }

    private void ManageCorruptionChangeLessThanStep(float corruptionStat, float corruptionLastValue, float diff)
    {
        int nextStep = (int)(corruptionStat / STEP_VALUE);
        bool isMovingPositive = Mathf.Abs(diff) < STEP_VALUE && nextStep > currentStep;
        bool isMovingNegative = Mathf.Abs(diff) < STEP_VALUE && nextStep < currentStep;

        if (isMovingPositive && diff > 0)
        {
            CorruptionUpgrade(corruptionStat);
        }
        else if (isMovingNegative && diff < 0)
        {
            BenedictionUpgrade(corruptionStat);
        }
        else if (isMovingPositive && corruptionStat < 0)
        {
            BenedictionDrawback(corruptionLastValue);
        }
        else if (isMovingNegative && corruptionStat > 0)
        {
            CorruptionDrawback(corruptionLastValue);
        }
    }

    private void ManageCorruptionChangeMoreThanStep(float corruptionStat, float corruptionLastValue, float diff)
    {
        float currentValue = corruptionLastValue;
        int stepDiff = Mathf.Abs((int)(diff / STEP_VALUE));
        int offset = diff > 0 ? STEP_VALUE : -STEP_VALUE;

        for (int i = 0; i < stepDiff; i++)
        {
            bool increaseAtStart = (diff > 0 && currentValue > 0) || (diff < 0 && currentValue <= 0);
            int currentDiff = (int)(corruptionStat - currentValue);
            if (increaseAtStart)
            {
                currentValue += offset;
            }

            if (currentValue <= 0 && currentDiff > 0)
            {
                BenedictionDrawback(currentValue);
            }
            else if (currentValue <= 0 && currentDiff < 0)
            {
                BenedictionUpgrade(currentValue);
            }
            else if (currentValue >= 0 && currentDiff > 0)
            {
                CorruptionUpgrade(currentValue);
            }
            else if (currentValue >= 0 && currentDiff < 0)
            {
                CorruptionDrawback(currentValue);
            }

            if (!increaseAtStart)
            {
                currentValue += offset;
            }
        }
    }

    private void CorruptionUpgrade(float corruptionStat)
    {
        currentStep++;
        if (corruptionStat >= Stats.GetMaxValue(Stat.CORRUPTION))
        {
            Stats.IncreaseValue(Stat.LIFE_STEAL, 0.15f);
            CanHealFromConsumables = false;
            playerController.SpecialAbility = new DamnationVeil();
            OnCorruptionMaxUpgrade?.Invoke(playerController.SpecialAbility);
        }
        else
        {
            Stats.IncreaseValue(Stat.ATK, 5f);
            Stats.DecreaseMaxValue(Stat.HP, 15f);
            Stats.DecreaseValue(Stat.HP, 15f);
        }
        playerController.LaunchUpgradeAnimation = true;
    }

    private void BenedictionUpgrade(float corruptionStat)
    {
        currentStep--;
        if (corruptionStat <= Stats.GetMinValue(Stat.CORRUPTION))
        {
            playerController.SpecialAbility = new DivineShield();
            OnBenedictionMaxUpgrade?.Invoke(playerController.SpecialAbility);
        }
        else
        {
            Stats.IncreaseMaxValue(Stat.HP, 15f);
            Stats.IncreaseValue(Stat.HP, 15f);
            Stats.DecreaseValue(Stat.ATK, 5f);
        }

        playerController.LaunchUpgradeAnimation = true;
    }

    private void BenedictionDrawback(float corruptionLastValue)
    {
        currentStep++;
        if (corruptionLastValue <= Stats.GetMinValue(Stat.CORRUPTION))
        {
            playerController.SpecialAbility = null;
            OnBenedictionMaxDrawback?.Invoke();
        }
        else
        {
            Stats.DecreaseMaxValue(Stat.HP, 15f);
            Stats.DecreaseValue(Stat.HP, 15f);
            Stats.IncreaseValue(Stat.ATK, 5f);
        }
        playerController.LaunchDrawbackAnimation = true;
    }

    private void CorruptionDrawback(float corruptionLastValue)
    {
        currentStep--;
        if (corruptionLastValue >= Stats.GetMaxValue(Stat.CORRUPTION))
        {
            Stats.DecreaseValue(Stat.LIFE_STEAL, 0.15f);
            CanHealFromConsumables = true;
            playerController.SpecialAbility = null;
            OnCorruptionMaxDrawback?.Invoke();
        }
        else
        {
            Stats.DecreaseValue(Stat.ATK, 5f);
            Stats.IncreaseMaxValue(Stat.HP, 15f);
            Stats.IncreaseValue(Stat.HP, 15f);
        }
        playerController.LaunchDrawbackAnimation = true;
    }
    #endregion
}