using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class PlayerInput : MonoBehaviour
{
    PlayerInputMap playerInputMap;
    PlayerController controller;
    PlayerInteractions m_interaction;
    HudHandler hudHandler;
    Animator animator;
    [SerializeField] GameObject weapon;
    CameraUtilities cameraUtilities;

    bool dashCooldown = false;
    readonly float DASH_COOLDOWN_TIME = 0.5f;
    float timerDash = 0f;

    //used to prevent that if you press both dash and attack button to do both at the same time
    float keyCooldown = 0f;
    bool triggerCooldownAttack = false;
    bool triggerCooldownDash = false;

    bool attackQueue = false;
    float chargedAttackTime = 0f;
    bool chargedAttackMax = false;
    readonly float CHARGED_ATTACK_MAX_TIME = 1f;
    public float ChargedAttackCoef { get; private set; } = 0f;
    public bool LaunchedChargedAttack { get; private set; } = false;

    readonly float ZOOM_DEZOOM_TIME = 0.2f;

    List<System.Func<float, float>> easeFuncs = new List<System.Func<float, float>>();

    public EasingFunctions.EaseName easeUnzoom;
    public EasingFunctions.EaseName easeZoom;
    public EasingFunctions.EaseName easeShake;
    void Awake()
    {
        playerInputMap = new PlayerInputMap();
        controller = GetComponent<PlayerController>();
        m_interaction = GetComponent<PlayerInteractions>();
        hudHandler = FindObjectOfType<HudHandler>();
        animator = GetComponent<Animator>();
        cameraUtilities = Camera.main.GetComponent<CameraUtilities>();
        EaseFuncsShitStorm();
    }

    private void EaseFuncsShitStorm()
    {
        easeFuncs.Add(EasingFunctions.EaseInBack);
        easeFuncs.Add(EasingFunctions.EaseInBounce);
        easeFuncs.Add(EasingFunctions.EaseInCirc);
        easeFuncs.Add(EasingFunctions.EaseInCubic);
        easeFuncs.Add(EasingFunctions.EaseInElastic);
        easeFuncs.Add(EasingFunctions.EaseInExpo);
        easeFuncs.Add(EasingFunctions.EaseInOutBack);
        easeFuncs.Add(EasingFunctions.EaseInOutBounce);
        easeFuncs.Add(EasingFunctions.EaseInOutCirc);
        easeFuncs.Add(EasingFunctions.EaseInOutCubic);
        easeFuncs.Add(EasingFunctions.EaseInOutElastic);
        easeFuncs.Add(EasingFunctions.EaseInOutExpo);
        easeFuncs.Add(EasingFunctions.EaseInOutQuad);
        easeFuncs.Add(EasingFunctions.EaseInOutQuart);
        easeFuncs.Add(EasingFunctions.EaseInOutQuint);
        easeFuncs.Add(EasingFunctions.EaseInOutSin);
        easeFuncs.Add(EasingFunctions.EaseInQuad);
        easeFuncs.Add(EasingFunctions.EaseInQuint);
        easeFuncs.Add(EasingFunctions.EaseInSin);
        easeFuncs.Add(EasingFunctions.EaseOutBack);
        easeFuncs.Add(EasingFunctions.EaseOutBounce);
        easeFuncs.Add(EasingFunctions.EaseOutCirc);
        easeFuncs.Add(EasingFunctions.EaseOutCubic);
        easeFuncs.Add(EasingFunctions.EaseOutElastic);
        easeFuncs.Add(EasingFunctions.EaseOutExpo);
        easeFuncs.Add(EasingFunctions.EaseOutQuad);
        easeFuncs.Add(EasingFunctions.EaseOutQuart);
        easeFuncs.Add(EasingFunctions.EaseOutQuint);
        easeFuncs.Add(EasingFunctions.EaseOutSin);
    }

    private void OnEnable()
    {
        playerInputMap.Enable();
        playerInputMap.Movement.Movement.performed += controller.ReadDirection;
        playerInputMap.Movement.Movement.canceled += controller.ReadDirection;
        playerInputMap.Attack.Attack.performed += Attack;
        playerInputMap.Dash.Dash.performed += Dash;
        playerInputMap.Interract.Interract.performed += m_interaction.Interract;
        playerInputMap.Attack.Throw.performed += ctx => ThrowOrRetrieveSpear();
        playerInputMap.Attack.ChargedAttack.performed += ChargedAttack;
        playerInputMap.Attack.ChargedAttack.canceled += ChargedAttackCanceled;
        playerInputMap.UI.ToggleMap.started += hudHandler.ToggleMap;
        playerInputMap.UI.Pause.started += ctx => hudHandler.TogglePause();
    }

    private void OnDisable()
    {
        playerInputMap.Movement.Movement.performed -= controller.ReadDirection;
        playerInputMap.Movement.Movement.canceled -= controller.ReadDirection;
        playerInputMap.Attack.Attack.performed -= Attack;
        playerInputMap.Dash.Dash.performed -= Dash;
        playerInputMap.Interract.Interract.performed -= m_interaction.Interract;
        playerInputMap.Attack.Throw.performed -= ctx => ThrowOrRetrieveSpear();
        playerInputMap.Attack.ChargedAttack.performed -= ChargedAttack;
        playerInputMap.Attack.ChargedAttack.canceled -= ChargedAttackCanceled;
        playerInputMap.UI.ToggleMap.started -= hudHandler.ToggleMap;
        playerInputMap.UI.Pause.started -= ctx => hudHandler.TogglePause();
        playerInputMap.Disable();
    }

    void Update()
    {
        //used so that you don't see the character running while in transition between the normal attack and the charged attack casting
        float magnitudeCoef = 10;
        if (LaunchedChargedAttack)
        {
            magnitudeCoef = 0f;
        }

        animator.SetFloat("Speed", controller.Direction.magnitude * magnitudeCoef, 0.1f, Time.deltaTime);
        animator.SetInteger("ComboCount", controller.ComboCount);

        if (triggerCooldownDash || triggerCooldownAttack)
        {
            keyCooldown += Time.deltaTime;
            if (keyCooldown > 0.2f)
            {
                triggerCooldownDash = false;
                triggerCooldownAttack = false;
                keyCooldown = 0f;
            }
        }

        if (dashCooldown)
        {
            timerDash += Time.deltaTime;
            if (timerDash >= DASH_COOLDOWN_TIME)
            {
                dashCooldown = false;
                timerDash = 0f;
            }
        }
    }

    public void ChargedAttack(InputAction.CallbackContext ctx)
    {
        if (CanCastChargedAttack())
        {
            animator.ResetTrigger("ChargedAttackCharging");
            animator.SetTrigger("ChargedAttackCharging");
            triggerCooldownAttack = true;
            controller.hero.State = (int)Entity.EntityState.ATTACK;
            LaunchedChargedAttack = true;
        }
    }

    public void StartChargedAttackCasting()
    {
        cameraUtilities.ChangeFov(cameraUtilities.defaultFOV + 0.2f, ZOOM_DEZOOM_TIME, easeFuncs[(int)easeUnzoom]);
        StartCoroutine(ChargedAttackCoroutine());
    }

    public void ChargedAttackCanceled(InputAction.CallbackContext ctx)
    {
        animator.ResetTrigger("ChargedAttackRelease");
        if (LaunchedChargedAttack)
        {
            StopAllCoroutines();
            controller.ComboCount = 0;
            animator.SetTrigger("ChargedAttackRelease");
        }
    }

    //used as animation event
    public void ChargedAttackRelease()
    {
        ChargedAttackCoef = chargedAttackMax ? 1 : chargedAttackTime / CHARGED_ATTACK_MAX_TIME;

        controller.AttackCollide(controller.chargedAttack, false);
        chargedAttackMax = false;
        chargedAttackTime = 0f;

        //apply visual effects and controller vibrations
        DeviceManager.Instance.ForceStopVibrations();
        DeviceManager.Instance.ApplyVibrations(0.3f * ChargedAttackCoef, 0.3f * ChargedAttackCoef, 0.25f);

        cameraUtilities.ShakeCamera(0.3f * ChargedAttackCoef, 0.25f, easeFuncs[(int)easeShake]);
        cameraUtilities.ChangeFov(cameraUtilities.defaultFOV, ZOOM_DEZOOM_TIME, easeFuncs[(int)easeZoom]);

        controller.PlayVFX(controller.chargedAttackVFX);
        AudioManager.Instance.PlaySound(controller.playerAttacks[0]);
    }

    public IEnumerator ChargedAttackCoroutine()
    {
        DeviceManager.Instance.ApplyVibrations(0.01f, 0.005f, float.MaxValue);
        while (chargedAttackTime < CHARGED_ATTACK_MAX_TIME)
        {
            chargedAttackTime += Time.deltaTime;
            yield return null;
        }

        DeviceManager.Instance.ForceStopVibrations();
        DeviceManager.Instance.ApplyVibrations(0.01f, 0.01f, float.MaxValue);
        chargedAttackMax = true;
        FloatingTextGenerator.CreateActionText(transform.position, "Max!");
    }

    public void Attack(InputAction.CallbackContext ctx)
    {
        if (CanAttack())
        {
            if (controller.hero.State == (int)Entity.EntityState.ATTACK)
            {
                attackQueue = true;
            }

            animator.ResetTrigger("BasicAttack");
            animator.SetTrigger("BasicAttack");
            triggerCooldownAttack = true;
            controller.hero.State = (int)Entity.EntityState.ATTACK;

        }
        else if (CanRetrieveSpear())
        {
            ThrowOrRetrieveSpear();
        }
    }

    public void Dash(InputAction.CallbackContext ctx)
    {
        if (CanDash())
        {
            controller.hero.State = (int)Hero.PlayerState.DASH;

            if (controller.Direction.x != 0f || controller.Direction.y != 0f)
            {
                controller.ModifyCamVectors(out Vector3 camRight, out Vector3 camForward);
                controller.DashDir = (camForward * controller.Direction.y + camRight * controller.Direction.x).normalized;
            }
            else
            {
                controller.DashDir = transform.forward;
            }
            controller.OverridePlayerRotation(Quaternion.LookRotation(controller.DashDir).eulerAngles.y, true);

            animator.ResetTrigger("Dash");
            animator.SetTrigger("Dash");
            triggerCooldownDash = true;
            dashCooldown = true;

            controller.PlayVFX2(controller.dashVFX);
            AudioManager.Instance.PlaySound(controller.playerDash);
        }
    }

    //used as animation event
    public void EndOfSpecialAnimation() //triggers for dash and hit animation to reset state
    {
        controller.ChangeState((int)Entity.EntityState.MOVE);
    }

    //used as animation event
    public void EndOfSpecialAnimationAttack() //triggers on attack animations to reset combo
    {
        if (!attackQueue)
        {
            if (!LaunchedChargedAttack)
            {
                controller.ChangeState((int)Entity.EntityState.MOVE);
            }
            controller.ComboCount = 0;
            animator.ResetTrigger("BasicAttack");
        }
        else
        {
            controller.ComboCount = (++controller.ComboCount) % controller.MAX_COMBO_COUNT;
        }

        attackQueue = false;

        foreach (NestedList<Collider> spearColliders in controller.spearAttacks)
        {
            foreach (Collider spearCollider in spearColliders.data)
            {
                spearCollider.gameObject.SetActive(false);
            }
        }
    }

    //used as animation event
    public void EndOfChargedAttack()
    {
        controller.ChangeState((int)Entity.EntityState.MOVE);
    }

    //used as animation event
    public void StartOfAttackAnimation()
    {
        controller.hero.OnAttack?.Invoke();
        controller.AttackCollide(controller.spearAttacks[controller.ComboCount].data);
        controller.PlayVFX2(controller.spearAttacksVFX[controller.ComboCount]);
        AudioManager.Instance.PlaySound(controller.playerAttacks[controller.ComboCount]);
    }

    public void ResetValuesInput()
    {
        attackQueue = false;
        LaunchedChargedAttack = false;
    }

    public void ThrowOrRetrieveSpear()
    {
        if (controller.hero.State == (int)Entity.EntityState.MOVE)
        {
            //rotate the player to mouse's direction if playing KB/mouse
            if (DeviceManager.Instance.IsPlayingKB())
            {
                controller.MouseOrientation();
            }
            else
            {
                controller.OrientationErrorMargin(controller.hero.Stats.GetValue(Stat.ATK_RANGE));
            }

            Spear spear = weapon.GetComponent<Spear>();

            // If spear is being thrown we can't recall this attack
            if (spear.IsThrowing) return;
            if (!spear.IsThrown)
            {
                spear.Throw(this.transform.position + this.transform.forward * controller.hero.Stats.GetValue(Stat.ATK_RANGE));
            }
            else
            {
                spear.Return();
            }

        }
    }

    private bool CanAttack()
    {
        return (controller.hero.State == (int)Entity.EntityState.MOVE ||
            (controller.hero.State == (int)Entity.EntityState.ATTACK && !attackQueue))
            && !triggerCooldownDash && !weapon.GetComponent<Spear>().IsThrown;
    }

    private bool CanCastChargedAttack()
    {
        return (controller.hero.State == (int)Entity.EntityState.MOVE 
            || controller.hero.State == (int)Entity.EntityState.ATTACK)
            && !triggerCooldownDash && !weapon.GetComponent<Spear>().IsThrown;
    }

    private bool CanRetrieveSpear()
    {
        return controller.hero.State == (int)Entity.EntityState.MOVE && !triggerCooldownDash && weapon.GetComponent<Spear>().IsThrown;
    }

    private bool CanDash()
    {
        return controller.hero.State == (int)Entity.EntityState.MOVE && !triggerCooldownAttack && !dashCooldown;
    }
}
