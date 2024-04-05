using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.VFX;

[RequireComponent(typeof(PlayerController))]
public class PlayerInput : MonoBehaviour
{
    Hero hero;
    Animator animator;
    HudHandler hudHandler;
    PlayerController controller;
    CameraUtilities cameraUtilities;
    PlayerInteractions playerInteractions;
    UnityEngine.InputSystem.PlayerInput playerInputMap;

    Coroutine dashCoroutine = null;
    Coroutine chargedAttackCoroutine = null;

    public Vector2 Direction { get; private set; } = Vector2.zero;
    public Vector3 DashDir { get; private set; } = Vector3.zero;

    //dash values
    bool dashInCooldown = false;
    readonly float DASH_COOLDOWN_TIME = 0.3f;

    //attack values
    bool attackQueue = false;
    float chargedAttackTime = 0f;
    bool chargedAttackMax = false;
    readonly float CHARGED_ATTACK_MAX_TIME = 1f;
    float chargedAttackScaleSize = 0f;
    float chargedAttackVFXMaxSize = 0f;
    public float ChargedAttackCoef { get; private set; } = 0f;
    public bool LaunchedChargedAttack { get; private set; } = false;

    readonly float ZOOM_DEZOOM_TIME = 0.2f;

    //used to cancel queued attacks when pressing another button during attack sequence
    bool ForceReturnToMove = false;

    [Header("Easing")]
    [SerializeField] EasingFunctions.EaseName easeUnzoom;
    [SerializeField] EasingFunctions.EaseName easeZoom;
    [SerializeField] EasingFunctions.EaseName easeShake;
    readonly List<System.Func<float, float>> easeFuncs = new();

    void Awake()
    {
        controller = GetComponent<PlayerController>();
        playerInteractions = GetComponent<PlayerInteractions>();
        hudHandler = FindObjectOfType<HudHandler>();
        animator = GetComponentInChildren<Animator>();
        cameraUtilities = Camera.main.GetComponent<CameraUtilities>();
    }

    private void Start()
    {
        playerInputMap = GetComponent<UnityEngine.InputSystem.PlayerInput>();
        EaseFuncsShitStorm();
        InputSetup();
        hero = GetComponent<Hero>();
        hero.OnChangeState += ResetForceReturnToMove;
        chargedAttackScaleSize = controller.ChargedAttack.gameObject.transform.localScale.x;
        chargedAttackVFXMaxSize = controller.ChargedAttackVFX.GetFloat("VFX Size");
        HudHandler.OnPause += DisableGameplayInputs;
        HudHandler.OnUnpause += EnableGameplayInputs;
    }

    private void OnDestroy()
    {
        hero.OnChangeState -= ResetForceReturnToMove;
        HudHandler.OnPause -= DisableGameplayInputs;
        HudHandler.OnUnpause -= EnableGameplayInputs;
        InputActionMap kbMap = playerInputMap.actions.FindActionMap("Keyboard", throwIfNotFound: true);
        InputManagement(kbMap, unsubscribe: true);
        InputActionMap gamepadMap = playerInputMap.actions.FindActionMap("Gamepad", throwIfNotFound: true);
        InputManagement(gamepadMap, unsubscribe: true);
    }

    #region Inputs

    private void ReadDirection(InputAction.CallbackContext ctx)
    {
        Direction = ctx.ReadValue<Vector2>().normalized;
    }

    private void ChargedAttack(InputAction.CallbackContext ctx)
    {
        if (!CanCastChargedAttack())
            return;

        animator.SetBool("ChargedAttackCasting", true);
        hero.State = (int)Entity.EntityState.ATTACK;
        LaunchedChargedAttack = true;
    }

    private void ChargedAttackCanceled(InputAction.CallbackContext ctx)
    {
        animator.ResetTrigger("ChargedAttackRelease");
        animator.SetBool("ChargedAttackCasting", false);

        if (!LaunchedChargedAttack)
            return;

        if ((chargedAttackTime / CHARGED_ATTACK_MAX_TIME) > 0.2f)
        {
            StopChargedAttackCoroutine();
            controller.ComboCount = 0;
            animator.SetTrigger("ChargedAttackRelease");
        }
        else
        {
            StopChargedAttackCoroutine();
            cameraUtilities.ChangeFov(cameraUtilities.defaultFOV, ZOOM_DEZOOM_TIME, easeFuncs[(int)easeZoom]);
            DeviceManager.Instance.ForceStopVibrations();
            hero.State = (int)Entity.EntityState.MOVE;
            controller.ResetValues();
        }
    }

    public void ChargedAttackRelease()
    {
        ChargedAttackCoef = chargedAttackMax ? 1 : chargedAttackTime / CHARGED_ATTACK_MAX_TIME;

        //set up collider and vfx size based on maintained time of charged attack
        Vector3 scale = controller.ChargedAttack.gameObject.transform.localScale;
        scale.x = ChargedAttackCoef * chargedAttackScaleSize;
        scale.z = ChargedAttackCoef * chargedAttackScaleSize;
        controller.ChargedAttack.gameObject.transform.localScale = scale;
        controller.ChargedAttackVFX.SetFloat("VFX Size", chargedAttackVFXMaxSize * 0.33f + ChargedAttackCoef * chargedAttackVFXMaxSize * 0.67f);

        controller.AttackCollide(controller.ChargedAttack, false);
        chargedAttackMax = false;
        chargedAttackTime = 0f;

        //apply visual effects and controller vibrations
        DeviceManager.Instance.ApplyVibrations(0.8f * ChargedAttackCoef, 0.8f * ChargedAttackCoef, 0.25f);

        cameraUtilities.ShakeCamera(0.3f * ChargedAttackCoef, 0.25f, easeFuncs[(int)easeShake]);
        cameraUtilities.ChangeFov(cameraUtilities.defaultFOV, ZOOM_DEZOOM_TIME, easeFuncs[(int)easeZoom]);

        controller.PlayVFX(controller.ChargedAttackVFX);
        AudioManager.Instance.PlaySound(controller.ChargedAttackReleaseSFX);
    }

    private IEnumerator ChargedAttackCoroutine()
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
        AudioManager.Instance.PlaySound(controller.ChargedAttackMaxSFX);
        yield return null;

        while (true)
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

    private void Attack(InputAction.CallbackContext ctx)
    {
        if (CanAttack())
        {
            if (hero.State == (int)Entity.EntityState.ATTACK)
            {
                attackQueue = true;
            }
            else
            {
                animator.ResetTrigger("BasicAttack");
                animator.SetTrigger("BasicAttack");
            }
            hero.State = (int)Entity.EntityState.ATTACK;
        }
        else if (CanRetrieveSpear())
        {
            ThrowOrRetrieveSpear(ctx);
        }
    }

    private void Dash(InputAction.CallbackContext ctx)
    {
        if (!CanDash())
            return;

        ResetComboWhenMoving(ctx);

        if (Direction != Vector2.zero)
        {
            controller.ModifyCamVectors(out Vector3 camRight, out Vector3 camForward);
            DashDir = (camForward * Direction.y + camRight * Direction.x).normalized;
        }
        else
        {
            DashDir = transform.forward;
        }
        controller.OverridePlayerRotation(Quaternion.LookRotation(DashDir).eulerAngles.y, true);

        animator.ResetTrigger("Dash");
        animator.SetTrigger("Dash");
    }

    private IEnumerator DashCoroutine()
    {
        dashInCooldown = true;
        yield return new WaitForSeconds(DASH_COOLDOWN_TIME);
        dashInCooldown = false;
        dashCoroutine = null;
    }

    private void ThrowOrRetrieveSpear(InputAction.CallbackContext ctx)
    {
        // If spear is being thrown we can't recall this attack
        if (hero.State != (int)Entity.EntityState.MOVE || controller.Spear.IsThrowing)
            return;

        if (DeviceManager.Instance.IsPlayingKB())
        {
            controller.MouseOrientation();
        }
        else
        {
            controller.JoystickOrientation();
            controller.OrientationErrorMargin(hero.Stats.GetValue(Stat.ATK_RANGE));
        }

        if (!controller.Spear.IsThrown)
        {
            Vector3 forward = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
            StartCoroutine(controller.Spear.Throw(this.transform.position + forward * hero.Stats.GetValue(Stat.ATK_RANGE)));
            controller.PlayVFX(controller.spearLaunchVFX);
        }
        else
        {
            StartCoroutine(controller.Spear.Return());
        }
    }

    private void Interract(InputAction.CallbackContext ctx)
    {
        Vector2 playerPos = transform.position.ToCameraOrientedVec2();
        IInterractable closestInteractable = playerInteractions.ItemsInRange.OrderBy(interactable =>
        {
            Vector2 itemPos = interactable.transform.position.ToCameraOrientedVec2();
            return Vector2.Distance(playerPos, itemPos);
        })
        .Select(interactable => interactable as IInterractable)
        .Where(interactable => interactable != null)
        .FirstOrDefault();

        closestInteractable?.Interract();
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
    public void StartOfDashAnimation()
    {
        controller.DashVFX.Play();
        hero.State = (int)Hero.PlayerState.DASH;
        AudioManager.Instance.PlaySound(controller.DashSFX);
    }

    public void EndOfDashAnimation()
    {
        RestartDashCoroutine();
        controller.DashVFX.Stop();
        hero.State = (int)Entity.EntityState.MOVE;
        controller.ResetValues();
    }

    public void StartChargedAttackCasting()
    {
        cameraUtilities.ChangeFov(cameraUtilities.defaultFOV + 0.2f, ZOOM_DEZOOM_TIME, easeFuncs[(int)easeUnzoom]);
        chargedAttackCoroutine = StartCoroutine(ChargedAttackCoroutine());
    }

    public void EndOfChargedAttack()
    {
        hero.State = (int)Entity.EntityState.MOVE;
        controller.ResetValues();
    }

    public void StartOfBasicAttack()
    {
        hero.OnAttack?.Invoke();
        controller.AttackCollide(controller.SpearAttacks[controller.ComboCount].data, false);

        //stop all VFX of the combo attacks to prevent them to overlap each other
        foreach (VisualEffect vfx in controller.SpearAttacksVFX)
        {
            vfx.Reinit();
            vfx.Stop();
        }

        controller.PlayVFX(controller.SpearAttacksVFX[controller.ComboCount]);
        AudioManager.Instance.PlaySound(controller.AttacksSFX[controller.ComboCount]);
    }

    /// <summary>
    /// Triggers on attack animations to reset combo.
    /// </summary>
    public void EndOfBasicAttack()
    {
        animator.ResetTrigger("BasicAttack");

        if (!attackQueue)
        {
            if (!LaunchedChargedAttack)
            {
                hero.State = (int)Entity.EntityState.MOVE;
                controller.ResetValues();
            }
            controller.ComboCount = 0;
        }
        else
        {
            animator.SetTrigger("BasicAttack");
            hero.State = (int)Entity.EntityState.ATTACK;
            controller.ComboCount = (++controller.ComboCount) % PlayerController.MAX_COMBO_COUNT;
        }

        attackQueue = false;

        foreach (NestedList<Collider> spearColliders in controller.SpearAttacks)
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
        return (hero.State == (int)Entity.EntityState.MOVE ||
            (hero.State == (int)Entity.EntityState.ATTACK && !attackQueue))
             && !controller.Spear.IsThrown && !ForceReturnToMove;
    }

    private bool CanCastChargedAttack()
    {
        return (hero.State == (int)Entity.EntityState.MOVE
            || hero.State == (int)Entity.EntityState.ATTACK)
            && !controller.Spear.IsThrown && !LaunchedChargedAttack;
    }

    private bool CanRetrieveSpear()
    {
        return hero.State == (int)Entity.EntityState.MOVE && controller.Spear.IsThrown;
    }

    private bool CanDash()
    {
        return (hero.State == (int)Entity.EntityState.MOVE
            || hero.State == (int)Entity.EntityState.ATTACK) && !dashInCooldown && !LaunchedChargedAttack;
    }

    private bool CanResetCombo()
    {
        return (
                (DeviceManager.Instance.IsPlayingKB() && Keyboard.current.anyKey.isPressed) ||
                (!DeviceManager.Instance.IsPlayingKB() && Gamepad.current.allControls.Any(x => x is ButtonControl button && x.IsPressed() && !x.synthetic))
               )
               && !playerInputMap.currentActionMap["BasicAttack"].IsPressed() && hero.State == (int)Entity.EntityState.ATTACK && !LaunchedChargedAttack;
    }

    #endregion

    #region Miscellaneous
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
        if (unsubscribe)
        {
            map["Movement"].performed -= ReadDirection;
            map["Movement"].started -= ResetComboWhenMoving;
            map["Movement"].canceled -= ReadDirection;
            map["BasicAttack"].performed -= Attack;
            map["Dash"].performed -= Dash;
            map["Interact"].performed -= Interract;
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
            map["Interact"].performed += Interract;
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

    private void DisableGameplayInputs()
    {
        playerInputMap.currentActionMap["Movement"].Disable();
        playerInputMap.currentActionMap["BasicAttack"].Disable();
        playerInputMap.currentActionMap["Dash"].Disable();
        playerInputMap.currentActionMap["Interact"].Disable();
        playerInputMap.currentActionMap["Spear"].Disable();
        playerInputMap.currentActionMap["ChargedAttack"].Disable();
    }

    private void EnableGameplayInputs()
    {
        playerInputMap.currentActionMap["Movement"].Enable();
        playerInputMap.currentActionMap["BasicAttack"].Enable();
        playerInputMap.currentActionMap["Dash"].Enable();
        playerInputMap.currentActionMap["Interact"].Enable();
        playerInputMap.currentActionMap["Spear"].Enable();
        playerInputMap.currentActionMap["ChargedAttack"].Enable();
    }

    private void ResetForceReturnToMove()
    {
        if (hero.State == (int)Entity.EntityState.MOVE)
        {
            ForceReturnToMove = false;
        }
    }
    public void ResetValuesInput()
    {
        StopChargedAttackCoroutine();
        attackQueue = false;
        LaunchedChargedAttack = false;
        chargedAttackMax = false;
        chargedAttackTime = 0f;
    }

    private void StopChargedAttackCoroutine()
    {
        if (chargedAttackCoroutine != null)
        {
            StopCoroutine(chargedAttackCoroutine);
            chargedAttackCoroutine = null;
        }
    }

    private void RestartDashCoroutine()
    {
        if (dashCoroutine != null)
        {
            StopCoroutine(dashCoroutine);
        }
        dashCoroutine = StartCoroutine(DashCoroutine());
    }
    #endregion
}
