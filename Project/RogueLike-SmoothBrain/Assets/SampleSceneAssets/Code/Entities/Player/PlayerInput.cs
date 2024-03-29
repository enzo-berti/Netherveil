using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

[RequireComponent(typeof(PlayerController))]
public class PlayerInput : MonoBehaviour
{
    UnityEngine.InputSystem.PlayerInput playerInputMap;
    PlayerController controller;
    PlayerInteractions m_interaction;
    HudHandler hudHandler;
    Animator animator;
    [SerializeField] Spear spear;
    CameraUtilities cameraUtilities;
    public Vector2 Direction { get; private set; } = Vector2.zero;

    bool dashCooldown = false;
    readonly float DASH_COOLDOWN_TIME = 0.5f;
    float timerDash = 0f;

    bool attackQueue = false;
    float chargedAttackTime = 0f;
    bool chargedAttackMax = false;
    readonly float CHARGED_ATTACK_MAX_TIME = 1f;
    public float ChargedAttackCoef { get; private set; } = 0f;
    public bool LaunchedChargedAttack { get; private set; } = false;

    readonly float ZOOM_DEZOOM_TIME = 0.2f;

    //used to cancel queued attacks when pressing another button during attack sequence
    bool ForceReturnToMove = false;

    readonly List<System.Func<float, float>> easeFuncs = new();
    public EasingFunctions.EaseName easeUnzoom;
    public EasingFunctions.EaseName easeZoom;
    public EasingFunctions.EaseName easeShake;
    void Awake()
    {
        controller = GetComponent<PlayerController>();
        m_interaction = GetComponent<PlayerInteractions>();
        hudHandler = FindObjectOfType<HudHandler>();
        animator = GetComponentInChildren<Animator>();
        cameraUtilities = Camera.main.GetComponent<CameraUtilities>();
    }

    private void Start()
    {
        playerInputMap = GetComponent<UnityEngine.InputSystem.PlayerInput>();
        EaseFuncsShitStorm();
        InputSetup();
        controller.hero.OnChangeState += ResetForceReturnToMove;
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

    private void InputSetup()
    {
        InputActionMap kbMap = playerInputMap.actions.FindActionMap("Keyboard", throwIfNotFound: true);
        InputManagement(kbMap, unsubscribe: false);
        InputActionMap gamepadMap = playerInputMap.actions.FindActionMap("Gamepad", throwIfNotFound: true);
        InputManagement(gamepadMap, unsubscribe: false);
    }

    void InputManagement(InputActionMap map, bool unsubscribe)
    {
        if(unsubscribe)
        {
            map["Movement"].performed -= ReadDirection;
            map["Movement"].started -= ResetComboWhenMoving;
            map["Movement"].canceled -= ReadDirection;
            map["BasicAttack"].performed -= Attack;
            map["Dash"].performed -= Dash;
            map["Interact"].performed -= m_interaction.Interract;
            map["Spear"].performed -= ThrowOrRetrieveSpear;
            map["ChargedAttack"].performed -= ChargedAttack;
            map["ChargedAttack"].canceled -= ChargedAttackCanceled;
            if (hudHandler != null)
            {
                map["ToggleMap"].performed -= hudHandler.ToggleMap;
                map["Pause"].started -= hudHandler.TogglePause;
            }
        }
        else
        {
            map["Movement"].performed += ReadDirection;
            map["Movement"].started += ResetComboWhenMoving;
            map["Movement"].canceled += ReadDirection;
            map["BasicAttack"].performed += Attack;
            map["Dash"].performed += Dash;
            map["Interact"].performed += m_interaction.Interract;
            map["Spear"].performed += ThrowOrRetrieveSpear;
            map["ChargedAttack"].performed += ChargedAttack;
            map["ChargedAttack"].canceled += ChargedAttackCanceled;
            if (hudHandler != null)
            {
                map["ToggleMap"].performed += hudHandler.ToggleMap;
                map["Pause"].started += hudHandler.TogglePause;
            }
        }
    }

    private void OnDestroy()
    {
        controller.hero.OnChangeState -= ResetForceReturnToMove;
        InputActionMap kbMap = playerInputMap.actions.FindActionMap("Keyboard", throwIfNotFound: true);
        InputManagement(kbMap, unsubscribe: true);
        InputActionMap gamepadMap = playerInputMap.actions.FindActionMap("Gamepad", throwIfNotFound: true);
        InputManagement(gamepadMap, unsubscribe: true);
    }

    void Update()
    {
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

    #region Inputs

    public void ReadDirection(InputAction.CallbackContext ctx)
    {
        Direction = ctx.ReadValue<Vector2>().normalized;
    }

    public void ChargedAttack(InputAction.CallbackContext ctx)
    {
        if (CanCastChargedAttack())
        {
            animator.SetBool("ChargedAttackCasting", true);
            controller.hero.State = (int)Entity.EntityState.ATTACK;
            LaunchedChargedAttack = true;
        }
    }

    public void ChargedAttackCanceled(InputAction.CallbackContext ctx)
    {
        animator.ResetTrigger("ChargedAttackRelease");
        animator.SetBool("ChargedAttackCasting", false);
        if (LaunchedChargedAttack && (chargedAttackTime/ CHARGED_ATTACK_MAX_TIME) > 0.2f)
        {
            StopAllCoroutines();
            controller.ComboCount = 0;
            animator.SetTrigger("ChargedAttackRelease");
        }
        else if (LaunchedChargedAttack && (chargedAttackTime / CHARGED_ATTACK_MAX_TIME) <= 0.2f)
        {
            StopAllCoroutines();
            cameraUtilities.ChangeFov(cameraUtilities.defaultFOV, ZOOM_DEZOOM_TIME, easeFuncs[(int)easeZoom]);
            DeviceManager.Instance.ForceStopVibrations();
            controller.hero.State = (int)Entity.EntityState.MOVE;
            controller.ResetValues();
        }
    }

    public void ChargedAttackRelease()
    {
        ChargedAttackCoef = chargedAttackMax ? 1 : chargedAttackTime / CHARGED_ATTACK_MAX_TIME;

        controller.AttackCollide(controller.chargedAttack, false);
        chargedAttackMax = false;
        chargedAttackTime = 0f;

        //apply visual effects and controller vibrations
        DeviceManager.Instance.ApplyVibrations(0.8f * ChargedAttackCoef, 0.8f * ChargedAttackCoef, 0.25f);

        cameraUtilities.ShakeCamera(0.3f * ChargedAttackCoef, 0.25f, easeFuncs[(int)easeShake]);
        cameraUtilities.ChangeFov(cameraUtilities.defaultFOV, ZOOM_DEZOOM_TIME, easeFuncs[(int)easeZoom]);

        controller.PlayVFX(controller.chargedAttackVFX);
        AudioManager.Instance.PlaySound(controller.chargedAttackReleaseSFX);
    }

    public IEnumerator ChargedAttackCoroutine()
    {
        DeviceManager.Instance.ApplyVibrationsInfinite(0f, 0.005f);
        
        while (chargedAttackTime < CHARGED_ATTACK_MAX_TIME)
        {
            chargedAttackTime += Time.deltaTime;
            if (DeviceManager.Instance.IsPlayingKB())
            {
                controller.MouseOrientation();
            }
            else
            {
                controller.JoystickOrientation();
            }
            yield return null;
        }

        DeviceManager.Instance.ApplyVibrationsInfinite(0.005f, 0.005f);
        chargedAttackMax = true;
        FloatingTextGenerator.CreateActionText(transform.position, "Max!");
        AudioManager.Instance.PlaySound(controller.chargedAttackMaxSFX);
        yield return null;

        while(true)
        {
            if (DeviceManager.Instance.IsPlayingKB())
            {
                controller.MouseOrientation();
            }
            else
            {
                controller.JoystickOrientation();
            }
            yield return null;
        }
    }

    public void Attack(InputAction.CallbackContext ctx)
    {
        if (CanAttack() && !ForceReturnToMove)
        {
            if (controller.hero.State == (int)Entity.EntityState.ATTACK)
            {
                attackQueue = true;
            }
            else
            {
                animator.ResetTrigger("BasicAttack");
                animator.SetTrigger("BasicAttack");
            }
            controller.hero.State = (int)Entity.EntityState.ATTACK;

        }
        else if (CanRetrieveSpear())
        {
            ThrowOrRetrieveSpear(ctx);
        }
    }

    //dash VFX is played in dashBehaviour script
    public void Dash(InputAction.CallbackContext ctx)
    {
        if (CanDash())
        {
            ResetComboWhenMoving(ctx);

            if (Direction != Vector2.zero)
            {
                controller.ModifyCamVectors(out Vector3 camRight, out Vector3 camForward);
                controller.DashDir = (camForward * Direction.y + camRight * Direction.x).normalized;
            }
            else
            {
                controller.DashDir = transform.forward;
            }
            controller.OverridePlayerRotation(Quaternion.LookRotation(controller.DashDir).eulerAngles.y, true);

            animator.ResetTrigger("Dash");
            animator.SetTrigger("Dash");
        }
    }

    public void ThrowOrRetrieveSpear(InputAction.CallbackContext ctx)
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
                controller.JoystickOrientation();
                controller.OrientationErrorMargin(controller.hero.Stats.GetValue(Stat.ATK_RANGE));
            }

            // If spear is being thrown we can't recall this attack
            if (spear.IsThrowing) return;
            if (!spear.IsThrown)
            {
                spear.Throw(this.transform.position + this.transform.forward * controller.hero.Stats.GetValue(Stat.ATK_RANGE));
                AudioManager.Instance.PlaySound(controller.throwSpearSFX);
            }
            else
            {
                AudioManager.Instance.PlaySound(controller.retrieveSpearSFX);
                spear.Return();
            }
        }
    }

    private void ResetComboWhenMoving(InputAction.CallbackContext ctx)
    {
        if (CanResetCombo())
        {
            ForceReturnToMove = true;
            controller.ResetValues();
        }
    }
    #endregion

    #region AnimationEvents
    public void StartChargedAttackCasting()
    {
        cameraUtilities.ChangeFov(cameraUtilities.defaultFOV + 0.2f, ZOOM_DEZOOM_TIME, easeFuncs[(int)easeUnzoom]);
        StartCoroutine(ChargedAttackCoroutine());
    }

    public void EndOfChargedAttack()
    {
        controller.hero.State = (int)Entity.EntityState.MOVE;
        controller.ResetValues();
    }

    public void EndOfDashAnimation() 
    {
        controller.hero.State = (int)Entity.EntityState.MOVE;
        controller.ResetValues();
    }

    public void StartOfBasicAttack()
    {
        controller.hero.OnAttack?.Invoke();
        controller.AttackCollide(controller.spearAttacks[controller.ComboCount].data, false);
        controller.PlayVFX(controller.spearAttacksVFX[controller.ComboCount]);
        AudioManager.Instance.PlaySound(controller.attacksSFX[controller.ComboCount]);
    }

    public void EndOfBasicAttack() //triggers on attack animations to reset combo
    {
        animator.ResetTrigger("BasicAttack");

        if (!attackQueue)
        {
            if (!LaunchedChargedAttack)
            {
                controller.hero.State = (int)Entity.EntityState.MOVE;
                controller.ResetValues();
            }
            controller.ComboCount = 0;
        }
        else
        {
            
            animator.SetTrigger("BasicAttack");
            controller.hero.State = (int)Entity.EntityState.ATTACK;
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
    #endregion

    #region InputConditions

    private bool CanAttack()
    {
        return (controller.hero.State == (int)Entity.EntityState.MOVE ||
            (controller.hero.State == (int)Entity.EntityState.ATTACK && !attackQueue))
             && !spear.IsThrown;
    }

    private bool CanCastChargedAttack()
    {
        return (controller.hero.State == (int)Entity.EntityState.MOVE 
            || controller.hero.State == (int)Entity.EntityState.ATTACK)
            && !spear.IsThrown && !LaunchedChargedAttack;
    }

    private bool CanRetrieveSpear()
    {
        return controller.hero.State == (int)Entity.EntityState.MOVE && spear.IsThrown;
    }

    private bool CanDash()
    {
        return (controller.hero.State == (int)Entity.EntityState.MOVE
            || controller.hero.State == (int)Entity.EntityState.ATTACK) && !dashCooldown && !LaunchedChargedAttack;
    }

    private bool CanResetCombo()
    {
        return (
                (DeviceManager.Instance.IsPlayingKB() && Keyboard.current.anyKey.isPressed) || 
                (!DeviceManager.Instance.IsPlayingKB() && Gamepad.current.allControls.Any(x => x is ButtonControl button && x.IsPressed() && !x.synthetic))
               )
               && !playerInputMap.currentActionMap["BasicAttack"].IsPressed() && controller.hero.State == (int)Entity.EntityState.ATTACK && !LaunchedChargedAttack;
    }

    #endregion

    #region Miscellaneous

    private void ResetForceReturnToMove()
    {
        if (controller.hero.State == (int)Entity.EntityState.MOVE)
        {
            ForceReturnToMove = false;
        }
    }
    public void ResetValuesInput()
    {
        StopAllCoroutines();
        attackQueue = false;
        LaunchedChargedAttack = false;
        chargedAttackMax = false;
        chargedAttackTime = 0f;
    }

    public void TriggerDashCooldown()
    {
        dashCooldown = true;
    }
    #endregion
}
