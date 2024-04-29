using Map;
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
        UPGRADING_STATS,
        MOTIONLESS
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

    public readonly int STEP_VALUE = 25;
    public readonly int BENEDICTION_MAX = -4;
    public readonly int CORRUPTION_MAX = 4;
    public readonly int MAX_INDEX_ALIGNMENT_TAB = 3;

    public bool CanHealFromConsumables { get; set; } = true;
    bool canLaunchUpgrade = false;

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
                if(!HudHandler.current.QuestHUD.QuestEnable)
                {
                    HudHandler.current.QuestHUD.Toggle();
                }
                value.AcceptQuest();
                OnQuestObtained?.Invoke();
            }
        }
    }

    [SerializeField] List<NestedList<GameObject>> CorruptionArmorsToActivatePerStep;
    [SerializeField] List<NestedList<GameObject>> BenedictionArmorsToActivatePerStep;
    [SerializeField] List<NestedList<GameObject>> NormalArmorsToActivatePerStep;

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

        RoomUtilities.allEnemiesDeadEvents += ChangeStatsBasedOnAlignment;
        RoomUtilities.allChestOpenEvents += ChangeStatsBasedOnAlignment;
        stats.onStatChange += UpgradePlayerStats;
    }

    private void OnDestroy()
    {
        RoomUtilities.allEnemiesDeadEvents -= ChangeStatsBasedOnAlignment;
        RoomUtilities.allChestOpenEvents -= ChangeStatsBasedOnAlignment;
        stats.onStatChange -= UpgradePlayerStats;
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
        if (IsInvincibleCount > 0)
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

    private void UpgradePlayerStats(Stat stat)
    {
        if (stat != Stat.CORRUPTION)
            return;

        canLaunchUpgrade = true;
    }

    public void ChangeStatsBasedOnAlignment()
    {
        int curStep = (int)(Stats.GetValue(Stat.CORRUPTION) / STEP_VALUE);
        int lastStep = (int)(Stats.GetLastValue(Stat.CORRUPTION) / STEP_VALUE);
        if (curStep == lastStep || !canLaunchUpgrade)
            return;

        TriggerAnimAndVFX(curStep, lastStep);
        ManageDrawbacks(lastStep);

        if (curStep < 0)
        {
            BenedictionUpgrade(curStep);
        }
        else if (curStep > 0)
        {
            CorruptionUpgrade(curStep);
        }
        else
        {
            ReactivateDefaultArmor();
        }
    }

    private void ManageDrawbacks(int lastStep)
    {
        for (int i = Mathf.Abs(lastStep); i > 0; i--)
        {
            if (lastStep < 0) // benediction drawbacks
            {
                if (i == Mathf.Abs(BENEDICTION_MAX))
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
            }
            else if (lastStep > 0) //corruption drawbacks
            {
                if (i == CORRUPTION_MAX)
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
            }
        }
    }

    private void TriggerAnimAndVFX(int curStep, int lastStep)
    {
        playerInput.DisableGameplayInputs();
        State = (int)Hero.PlayerState.UPGRADING_STATS;
        canLaunchUpgrade = false;

        bool corruptionUpgradeOnly = curStep > 0 && lastStep >= 0 && lastStep < curStep;
        bool benedictionUpgradeOnly = curStep < 0 && lastStep <= 0 && lastStep > curStep;

        bool hasbenedictionDrawbackNegativeToPositive = curStep >= 0 && lastStep < 0;
        bool hasbenedictionDrawbackNegativeOnly = curStep < 0 && lastStep <= 0 && lastStep < curStep;

        bool hascorruptionDrawbackPositiveToNegative = curStep <= 0 && lastStep > 0;
        bool hascorruptionDrawbackPositiveOnly = curStep > 0 && lastStep >= 0 && lastStep > curStep;

        if (corruptionUpgradeOnly)
        {
            playerController.corruptionUpgradeVFX.GetComponent<VFXStopper>().PlayVFX();
            AudioManager.Instance.PlaySound(playerController.StepUpgradeSFX);
            animator.ResetTrigger("CorruptionUpgrade");
            animator.SetTrigger("CorruptionUpgrade");
        }
        else if (benedictionUpgradeOnly)
        {
            playerController.benedictionUpgradeVFX.GetComponent<VFXStopper>().PlayVFX();
            AudioManager.Instance.PlaySound(playerController.StepUpgradeSFX);
            animator.ResetTrigger("BenedictionUpgrade");
            animator.SetTrigger("BenedictionUpgrade");
        }
        else if (hascorruptionDrawbackPositiveToNegative || hascorruptionDrawbackPositiveOnly)
        {
            playerController.corruptionDrawbackVFX.GetComponent<VFXStopper>().PlayVFX();
            AudioManager.Instance.PlaySound(playerController.StepDowngradeSFX);

            if (hascorruptionDrawbackPositiveToNegative && curStep < 0)
            {
                playerController.benedictionUpgradeVFX.GetComponent<VFXStopper>().PlayVFX();
                AudioManager.Instance.PlaySound(playerController.StepUpgradeSFX);
                animator.ResetTrigger("BenedictionUpgrade");
                animator.SetTrigger("BenedictionUpgrade");
            }
            else
            {
                playerInput.EnableGameplayInputs();
                State = (int)Entity.EntityState.MOVE;
            }
        }
        else if (hasbenedictionDrawbackNegativeToPositive || hasbenedictionDrawbackNegativeOnly)
        {
            playerController.benedictionDrawbackVFX.GetComponent<VFXStopper>().PlayVFX();
            AudioManager.Instance.PlaySound(playerController.StepDowngradeSFX);
            if (hasbenedictionDrawbackNegativeToPositive && curStep > 0)
            {
                playerController.corruptionUpgradeVFX.GetComponent<VFXStopper>().PlayVFX();
                AudioManager.Instance.PlaySound(playerController.StepUpgradeSFX);
                animator.ResetTrigger("CorruptionUpgrade");
                animator.SetTrigger("CorruptionUpgrade");
            }
            else
            {
                playerInput.EnableGameplayInputs();
                State = (int)Entity.EntityState.MOVE;
            }
        }
    }

    private void BenedictionUpgrade(int curStep)
    {
        for (int i = 0; i < Mathf.Abs(curStep); i++)
        {
            if (i == MAX_INDEX_ALIGNMENT_TAB)
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

            foreach (GameObject armorPiece in BenedictionArmorsToActivatePerStep[i].data)
            {
                armorPiece.SetActive(true);
            }
            foreach (GameObject armorPiece in NormalArmorsToActivatePerStep[i].data)
            {
                armorPiece.SetActive(false);
            }
        }
    }

    private void CorruptionUpgrade(int curStep)
    {
        for (int i = 0; i < curStep; i++)
        {
            if (i == MAX_INDEX_ALIGNMENT_TAB)
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

            foreach (GameObject armorPiece in CorruptionArmorsToActivatePerStep[i].data)
            {
                armorPiece.SetActive(true);
            }
            foreach (GameObject armorPiece in NormalArmorsToActivatePerStep[i].data)
            {
                armorPiece.SetActive(false);
            }
        }
    }

    private void ReactivateDefaultArmor()
    {
        foreach (NestedList<GameObject> armorPiecesList in CorruptionArmorsToActivatePerStep)
        {
            foreach (GameObject armorPiece in armorPiecesList.data)
            {
                armorPiece.SetActive(false);
            }
        }

        foreach (NestedList<GameObject> armorPiecesList in BenedictionArmorsToActivatePerStep)
        {
            foreach (GameObject armorPiece in armorPiecesList.data)
            {
                armorPiece.SetActive(false);
            }
        }

        foreach (NestedList<GameObject> armorPiecesList in NormalArmorsToActivatePerStep)
        {
            foreach (GameObject armorPiece in armorPiecesList.data)
            {
                armorPiece.SetActive(true);
            }
        }
    }

    public void DebugCallLaunchUpgrade()
    {
        canLaunchUpgrade = true;
    }
    #endregion
}