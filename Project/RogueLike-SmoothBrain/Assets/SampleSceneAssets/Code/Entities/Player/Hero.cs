using Fountain;
using PostProcessingEffects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Hero : Entity, IDamageable, IAttacker, IBlastable
{
    public enum PlayerState : int
    {
        DASH = EntityState.NB,
        KNOCKBACK,
        UPGRADING_STATS,
        MOTIONLESS
    }

    public static Color corruptionColor = new Color(0.62f, 0.34f, 0.76f, 1.0f);
    public static Color corruptionColor2 = new Color(0.4f, 0.08f, 0.53f);
    public static Color benedictionColor = Color.yellow;
    public static Color benedictionColor2 = new Color(0.89f, 0.75f, 0.14f);

    Animator animator;
    PlayerInput playerInput;
    PlayerController playerController;
    public Inventory Inventory { get; private set; } = new Inventory();

    private event Action<IDamageable> onKill;
    private event IAttacker.AttackDelegate onAttack;
    private event IAttacker.HitDelegate onAttackHit;
    public event Action<int, IAttacker> OnTakeDamage;
    public event Action<IDamageable, IAttacker> OnBasicAttack;
    public event Action<IDamageable, IAttacker> OnDashAttack;
    public event Action<IDamageable, IAttacker> OnSpearAttack;
    public event Action<IDamageable, IAttacker> OnChargedAttack;
    public event Action<IDamageable, IAttacker> OnFinisherAttack;
    public event Action OnQuestObtained;
    public event Action OnQuestFinished;
    public event Action<ISpecialAbility> OnBenedictionMaxUpgrade;
    public event Action<ISpecialAbility> OnCorruptionMaxUpgrade;
    public event Action OnBenedictionMaxDrawback;
    public event Action OnCorruptionMaxDrawback;

    public delegate void OnBeforeApplyDamagesDelegate(ref int damages, IDamageable target);
    public event OnBeforeApplyDamagesDelegate OnBeforeApplyDamages;

    public IAttacker.AttackDelegate OnAttack { get => onAttack; set => onAttack = value; }
    public IAttacker.HitDelegate OnAttackHit { get => onAttackHit; set => onAttackHit = value; }
    public Action<IDamageable> OnKill { get => onKill; set => onKill = value; }

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
                if (HudHandler.current.QuestHUD.QuestEnable)
                {
                    HudHandler.current.QuestHUD.Toggle();
                }
                OnQuestFinished?.Invoke();
            }
            else
            {
                if (!HudHandler.current.QuestHUD.QuestEnable)
                {
                    HudHandler.current.QuestHUD.Toggle();
                }
                value.AcceptQuest();
                value.LateAcceptQuest();
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

        

        OnKill += ApplyLifeSteal;
        OnAttackHit += CorruptionNerf;
        FountainInteraction.onAddBenedictionCorruption += ChangeStatsBasedOnAlignment;
        Quest.OnQuestFinished += ChangeStatsBasedOnAlignment;
        Item.OnLateRetrieved += ChangeStatsBasedOnAlignment;
        stats.onStatChange += UpgradePlayerStats;
        //OnDeath += Inventory.RemoveAllItems;
    }

    private void OnDestroy()
    {
        FountainInteraction.onAddBenedictionCorruption -= ChangeStatsBasedOnAlignment;
        Quest.OnQuestFinished -= ChangeStatsBasedOnAlignment;
        Item.OnLateRetrieved -= ChangeStatsBasedOnAlignment;
        stats.onStatChange -= UpgradePlayerStats;

       //Inventory.RemoveAllItems(Vector3.zero);
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
                animator.ResetTrigger(playerController.ChargedAttackReleaseHash);
                animator.SetBool(playerController.ChargedAttackCastingHash, false);
                animator.ResetTrigger(playerController.BasicAttackHash);
                State = (int)Entity.EntityState.MOVE;
                AudioManager.Instance.PlaySound(playerController.HitSFX);
                FloatingTextGenerator.CreateEffectDamageText(_value, transform.position, Color.red);
                PostProcessingEffectManager.current.Play(Effect.Hit, false);
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
        animator.SetBool(playerController.IsKnockbackHash, false);
        animator.SetBool(playerController.IsDeadHash, true);
    }

    public void Attack(IDamageable damageable, int additionalDamages = 0)
    {
        int damages = (int)stats.GetValueWithoutCoeff(Stat.ATK);

        if (playerInput.LaunchedChargedAttack)
        {
            damages += (int)(playerController.CHARGED_ATTACK_DAMAGES * playerInput.ChargedAttackCoef);
            ApplyKnockback(damageable, this, stats.GetValue(Stat.KNOCKBACK_DISTANCE) * playerController.CHARGED_ATTACK_KNOCKBACK_COEFF * playerInput.ChargedAttackCoef,
                stats.GetValue(Stat.KNOCKBACK_COEFF) * playerController.CHARGED_ATTACK_KNOCKBACK_COEFF * playerInput.ChargedAttackCoef);
            OnChargedAttack?.Invoke(damageable, this);
        }
        else if (playerController.ComboCount == playerController.MAX_COMBO_COUNT - 1)
        {
            damages += playerController.FINISHER_DAMAGES;
            DeviceManager.Instance.ApplyVibrations(0.1f, 0f, 0.1f);
            ApplyKnockback(damageable, this);
            OnFinisherAttack?.Invoke(damageable, this);
        }
        else if (playerController.Spear.IsThrowing || playerController.Spear.IsThrown)
        {
            damages += playerController.SPEAR_DAMAGES;
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

    private void ApplyLifeSteal(IDamageable damageable)
    {
        int lifeIncreasedValue = (int)(Stats.GetValue(Stat.LIFE_STEAL) * (damageable as Mobs).Stats.GetMaxValue(Stat.HP) * 0.75f);
        lifeIncreasedValue = (int)(lifeIncreasedValue * Stats.GetValue(Stat.HEAL_COEFF));
        if (lifeIncreasedValue > 0 && (damageable as Mobs) != null && !(damageable as Mobs).IsSpawning)
        {
            AudioManager.Instance.PlaySound(playerController.HealSFX, transform.position);
            FloatingTextGenerator.CreateHealText(lifeIncreasedValue, transform.position);
            Stats.IncreaseValue(Stat.HP, lifeIncreasedValue);
        }
    }

    private void CorruptionNerf(IDamageable damageable, IAttacker attacker)
    {
        if(Stats.GetValue(Stat.CORRUPTION) >= STEP_VALUE && !(damageable as Mobs).IsSpawning)
        {
            int value = (int)(stats.GetMaxValue(Stat.HP) * 0.01f);
            stats.DecreaseValue(Stat.HP, value);
            AudioManager.Instance.PlaySound(playerController.HitSFX);
            FloatingTextGenerator.CreateEffectDamageText(value, transform.position, Color.red);
            playerController.HitVFX.Play();
            PostProcessingEffectManager.current.Play(Effect.Hit, false);
        }
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
        AudioManager.Instance.PlaySound(AudioManager.Instance.LostLevelSFX,transform.position);
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
                    Stats.DecreaseValue(Stat.LIFE_STEAL, 0.05f);
                    CanHealFromConsumables = true;
                    playerController.SpecialAbility = null;
                    OnCorruptionMaxDrawback?.Invoke();
                }
                else
                {
                    Stats.DecreaseValue(Stat.LIFE_STEAL, 0.05f);
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
            animator.ResetTrigger(playerController.CorruptionUpgradeHash);
            animator.SetTrigger(playerController.CorruptionUpgradeHash);
            animator.ResetTrigger(playerController.PouringBloodHash);
        }
        else if (benedictionUpgradeOnly)
        {
            playerController.benedictionUpgradeVFX.GetComponent<VFXStopper>().PlayVFX();
            AudioManager.Instance.PlaySound(playerController.StepUpgradeSFX);
            animator.ResetTrigger(playerController.BenedictionUpgradeHash);
            animator.SetTrigger(playerController.BenedictionUpgradeHash);
            animator.ResetTrigger(playerController.PouringBloodHash);
        }
        else if (hascorruptionDrawbackPositiveToNegative || hascorruptionDrawbackPositiveOnly)
        {
            playerController.DrawbackVFX.SetBool("Corruption", true);
            playerController.DrawbackVFX.Play();
            AudioManager.Instance.PlaySound(playerController.StepDowngradeSFX);

            if (hascorruptionDrawbackPositiveToNegative && curStep < 0)
            {
                playerController.benedictionUpgradeVFX.GetComponent<VFXStopper>().PlayVFX();
                AudioManager.Instance.PlaySound(playerController.StepUpgradeSFX);
                animator.ResetTrigger(playerController.BenedictionUpgradeHash);
                animator.SetTrigger(playerController.BenedictionUpgradeHash);
                animator.ResetTrigger(playerController.PouringBloodHash);
            }
            else
            {
                playerInput.EnableGameplayInputs();
                State = (int)Entity.EntityState.MOVE;
            }
        }
        else if (hasbenedictionDrawbackNegativeToPositive || hasbenedictionDrawbackNegativeOnly)
        {
            playerController.DrawbackVFX.SetBool("Corruption", false);
            playerController.DrawbackVFX.Play();
            AudioManager.Instance.PlaySound(playerController.StepDowngradeSFX);
            if (hasbenedictionDrawbackNegativeToPositive && curStep > 0)
            {
                playerController.corruptionUpgradeVFX.GetComponent<VFXStopper>().PlayVFX();
                AudioManager.Instance.PlaySound(playerController.StepUpgradeSFX);
                animator.ResetTrigger(playerController.CorruptionUpgradeHash);
                animator.SetTrigger(playerController.CorruptionUpgradeHash);
                animator.ResetTrigger(playerController.PouringBloodHash);
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
        AudioManager.Instance.PlaySound(AudioManager.Instance.GainLevelBenedictionSFX, transform.position);
        for (int i = 0; i < Mathf.Abs(curStep); i++)
        {
            if (i == MAX_INDEX_ALIGNMENT_TAB)
            {
                playerController.SpecialAbility = new DivineShield();
                OnBenedictionMaxUpgrade?.Invoke(playerController.SpecialAbility);
                StartCoroutine(OpenSpecialAbilityTab());
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
        AudioManager.Instance.PlaySound(AudioManager.Instance.GainLevelCorruptionSFX,transform.position);
        for (int i = 0; i < curStep; i++)
        {
            if (i == MAX_INDEX_ALIGNMENT_TAB)
            {
                Stats.IncreaseValue(Stat.LIFE_STEAL, 0.05f);
                CanHealFromConsumables = false;
                playerController.SpecialAbility = new DamnationVeil();
                OnCorruptionMaxUpgrade?.Invoke(playerController.SpecialAbility);
                StartCoroutine(OpenSpecialAbilityTab());
            }
            else
            {
                Stats.IncreaseValue(Stat.LIFE_STEAL, 0.05f);
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

    private IEnumerator OpenSpecialAbilityTab()
    {
        if (Stats.GetValue(Stat.CORRUPTION) == Stats.GetMaxValue(Stat.CORRUPTION))
        {
            yield return new WaitForSeconds(playerController.corruptionUpgradeVFX.GetComponent<VFXStopper>().Duration);
            HudHandler.current.DescriptionTab.SetTab("Damnation Veil", "On activation, creates a damnation zone that applies the damnation effect that doubles the damages received to all enemies touched by the zone.", GameResources.Get<VideoClip>("CorruptionVideo"), GameResources.Get<Sprite>("SpecialAbilityBackgroundCoruption"));
            HudHandler.current.DescriptionTab.OpenTab();
        }
        else if (Stats.GetValue(Stat.CORRUPTION) == Stats.GetMinValue(Stat.CORRUPTION))
        {
            yield return new WaitForSeconds(playerController.benedictionUpgradeVFX.GetComponent<VFXStopper>().Duration);
            HudHandler.current.DescriptionTab.SetTab("Divine Shield", "On activation, creates a shield around you that nullifies damages for a small amount of time.", GameResources.Get<VideoClip>("BenedictionVideo"), GameResources.Get<Sprite>("SpecialAbilityBackgroundBenediction"));
            HudHandler.current.DescriptionTab.OpenTab();
        }
    }

    public static void CallCorruptionBenedictionText(int value)
    {
        FloatingTextGenerator.CreateActionText(Utilities.Player.transform.position, (value < 0 ? "-" : "+") + $"{Mathf.Abs(value)}" + (value < 0 ? " Benediction" : " Corruption"),
            value < 0 ? benedictionColor : corruptionColor);
    }

    public void DebugCallLaunchUpgrade()
    {
        canLaunchUpgrade = true;
    }
    #endregion
}