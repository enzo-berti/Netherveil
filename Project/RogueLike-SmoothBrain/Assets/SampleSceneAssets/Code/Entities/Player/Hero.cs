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

    readonly float BENEDICTION_HP_STEP = 25f;
    readonly float BENEDICTION_HEAL_COEF_STEP = 1f;
    readonly float CORRUPTION_ATK_STEP = 2f;
    readonly float CORRUPTION_HP_STEP = 25f;
    readonly float CORRUPTION_LIFESTEAL_STEP = 0.15f;
    readonly float CORRUPTION_TAKE_DAMAGE_COEF_STEP = 0.25f;

    readonly float MAX_LIFESTEAL_HP_PERCENTAGE = 0.75f;
    float takeDamageCoeff = 1f;

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

        Stats.DecreaseValue(Stat.HP, (int)(_value * takeDamageCoeff), false);

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
                FloatingTextGenerator.CreateEffectDamageText((int)(_value * takeDamageCoeff), transform.position, Color.red);
                PostProcessingEffectManager.current.Play(Effect.Hit, false);
                playerController.HitVFX.Play();
            }

            OnTakeDamage?.Invoke((int)(_value * takeDamageCoeff), attacker);
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
        int lifeIncreasedValue = (int)(Stats.GetValue(Stat.LIFE_STEAL) * (damageable as Mobs).Stats.GetMaxValue(Stat.HP) * MAX_LIFESTEAL_HP_PERCENTAGE);
        lifeIncreasedValue = (int)(lifeIncreasedValue * Stats.GetValue(Stat.HEAL_COEFF));
        if (lifeIncreasedValue > 0 && (damageable as Mobs) != null && !(damageable as Mobs).IsSpawning)
        {
            AudioManager.Instance.PlaySound(playerController.HealSFX, transform.position);
            FloatingTextGenerator.CreateHealText(lifeIncreasedValue, transform.position);
            Stats.IncreaseValue(Stat.HP, lifeIncreasedValue);
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
            ManageBenedictionUpgrade(curStep);
        }
        else if (curStep > 0)
        {
            ManageCorruptionUpgrade(curStep);
        }
        else
        {
            ReactivateDefaultArmor();
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

    private void ManageDrawbacks(int lastStep)
    {
        bool playedSound = false;   
        for (int i = Mathf.Abs(lastStep); i > 0; i--)
        {
            if(!playedSound)
            {
                AudioManager.Instance.PlaySound(AudioManager.Instance.LostLevelSFX, transform.position);
                playedSound = true;
            }

            if (lastStep < 0) // benediction drawbacks
            {
                if (i == Mathf.Abs(BENEDICTION_MAX))
                {
                    BenedictionMaxDrawback();
                }
                else
                {
                    BenedictionDrawback();
                }
            }
            else if (lastStep > 0) //corruption drawbacks
            {
                if (i == CORRUPTION_MAX)
                {
                    CorruptionMaxDrawback();
                }
                else
                {
                    CorruptionDrawback();
                }
            }
        }
    }

    private void ManageBenedictionUpgrade(int curStep)
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.GainLevelBenedictionSFX, transform.position);
        for (int i = 0; i < Mathf.Abs(curStep); i++)
        {
            if (i == MAX_INDEX_ALIGNMENT_TAB)
            {
                BenedictionMaxUpgrade();
            }
            else
            {
                BenedictionUpgrade();
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

    private void ManageCorruptionUpgrade(int curStep)
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.GainLevelCorruptionSFX, transform.position);
        for (int i = 0; i < curStep; i++)
        {
            if (i == MAX_INDEX_ALIGNMENT_TAB)
            {
                CorruptionMaxUpgrade();
            }
            else
            {
                CorruptionUpgrade();
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

    private void BenedictionMaxUpgrade()
    {
        playerController.SpecialAbility = new DivineShield();
        stats.IncreaseValue(Stat.HEAL_COEFF, BENEDICTION_HEAL_COEF_STEP, false);
        BenedictionUpgrade();
        OnBenedictionMaxUpgrade?.Invoke(playerController.SpecialAbility);
        StartCoroutine(OpenSpecialAbilityTab());
    }

    private void BenedictionUpgrade()
    {
        Stats.IncreaseMaxValue(Stat.HP, BENEDICTION_HP_STEP);
        Stats.IncreaseValue(Stat.HP, BENEDICTION_HP_STEP);
        //Stats.DecreaseCoeffValue(Stat.ATK, BENEDICTION_ATK_COEF_STEP);
    }

    private void BenedictionMaxDrawback()
    {
        playerController.SpecialAbility = null;
        stats.DecreaseValue(Stat.HEAL_COEFF, BENEDICTION_HEAL_COEF_STEP, false);
        BenedictionDrawback();
        OnBenedictionMaxDrawback?.Invoke();
    }

    private void BenedictionDrawback()
    {
        Stats.DecreaseMaxValue(Stat.HP, BENEDICTION_HP_STEP);
        Stats.DecreaseValue(Stat.HP, BENEDICTION_HP_STEP);
        //Stats.IncreaseCoeffValue(Stat.ATK, BENEDICTION_ATK_COEF_STEP);
    }

    private void CorruptionMaxUpgrade()
    {
        CorruptionUpgrade();
        CanHealFromConsumables = false;
        playerController.SpecialAbility = new DamnationVeil();
        OnCorruptionMaxUpgrade?.Invoke(playerController.SpecialAbility);
        StartCoroutine(OpenSpecialAbilityTab());
    }

    private void CorruptionUpgrade()
    {
        takeDamageCoeff += CORRUPTION_TAKE_DAMAGE_COEF_STEP;
        Stats.IncreaseValue(Stat.LIFE_STEAL, CORRUPTION_LIFESTEAL_STEP);
        Stats.IncreaseValue(Stat.ATK, CORRUPTION_ATK_STEP);
        Stats.DecreaseMaxValue(Stat.HP, CORRUPTION_HP_STEP);
        Stats.DecreaseValue(Stat.HP, CORRUPTION_HP_STEP);
    }

    private void CorruptionMaxDrawback()
    {
        CorruptionDrawback();
        CanHealFromConsumables = true;
        playerController.SpecialAbility = null;
        OnCorruptionMaxDrawback?.Invoke();
    }

    private void CorruptionDrawback()
    {
        Stats.DecreaseValue(Stat.LIFE_STEAL, CORRUPTION_LIFESTEAL_STEP);
        takeDamageCoeff -= CORRUPTION_TAKE_DAMAGE_COEF_STEP;
        Stats.DecreaseValue(Stat.ATK, CORRUPTION_ATK_STEP);
        Stats.IncreaseMaxValue(Stat.HP, CORRUPTION_HP_STEP);
        Stats.IncreaseValue(Stat.HP, CORRUPTION_HP_STEP);
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

            HudHandler.current.DescriptionTab.SetTab("<color=#44197c><b>Damnation Veil</b></color>", 
                "On activation, creates a <color=purple><b>damnation zone</b></color> that applies the <color=purple><b>damnation effect</b></color> " +
                "that <color=red>doubles the damages</color> received to all enemies touched by the zone.",
                GameResources.Get<VideoClip>("CorruptionVideo"), 
                GameResources.Get<Sprite>("SpecialAbilityBackgroundCoruption"));

            HudHandler.current.DescriptionTab.OpenTab();
        }
        else if (Stats.GetValue(Stat.CORRUPTION) == Stats.GetMinValue(Stat.CORRUPTION))
        {
            yield return new WaitForSeconds(playerController.benedictionUpgradeVFX.GetComponent<VFXStopper>().Duration);

            HudHandler.current.DescriptionTab.SetTab("<color=yellow><b>Divine Shield</b></color>",
                "On activation, creates a <color=#a52a2aff><b>shield</b></color> around you that <color=#a52a2aff><b>nullifies damages</b></color> for a small amount of time.", 
                GameResources.Get<VideoClip>("BenedictionVideo"), 
                GameResources.Get<Sprite>("SpecialAbilityBackgroundBenediction"));
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