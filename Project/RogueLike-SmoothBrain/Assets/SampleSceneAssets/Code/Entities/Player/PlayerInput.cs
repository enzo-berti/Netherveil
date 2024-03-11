using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

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
    public bool LaunchedAttack { get; private set; } = false;
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
    }

    void Start()
    {
        controller = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        cameraUtilities = Camera.main.GetComponent<CameraUtilities>();

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
    }

    private void OnDisable()
    {
        playerInputMap.Disable();
        playerInputMap.Movement.Movement.performed -= controller.ReadDirection;
        playerInputMap.Movement.Movement.canceled -= controller.ReadDirection;
        playerInputMap.Attack.Attack.performed -= Attack;
        playerInputMap.Dash.Dash.performed -= Dash;
        playerInputMap.Interract.Interract.performed -= m_interaction.Interract;
        playerInputMap.Attack.Throw.performed -= ctx => ThrowOrRetrieveSpear();
        playerInputMap.Attack.ChargedAttack.performed -= ChargedAttack;
        playerInputMap.Attack.ChargedAttack.canceled -= ChargedAttackCanceled;
        playerInputMap.UI.ToggleMap.started -= hudHandler.ToggleMap;
    }

    void Update()
    {
        //used so that you don't see the character running while in transition between the normal attack and the charged attack casting
        float magnitudeCoef = 10;
        if(LaunchedChargedAttack)
        {
            magnitudeCoef = 0f;
        }

        animator.SetFloat("Speed", controller.Direction.magnitude * magnitudeCoef, 0.1f, Time.deltaTime);
        animator.SetBool("BasicAttacking", LaunchedAttack);
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
        if ((controller.hero.State == (int)Entity.EntityState.MOVE || controller.hero.State == (int)Entity.EntityState.ATTACK)
    && !triggerCooldownDash && !weapon.GetComponent<Spear>().IsThrown)
        {
            LaunchedAttack = false;
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
        DeviceManager.Instance.ForceStopVibrations();
        ChargedAttackCoef = chargedAttackMax ? 1 : chargedAttackTime / CHARGED_ATTACK_MAX_TIME;

        cameraUtilities.ShakeCamera(0.3f * ChargedAttackCoef, 0.25f, easeFuncs[(int)easeShake]);
        DeviceManager.Instance.ApplyVibrations(0.3f * ChargedAttackCoef, 0.3f * ChargedAttackCoef, 0.25f);
        cameraUtilities.ChangeFov(cameraUtilities.defaultFOV, ZOOM_DEZOOM_TIME, easeFuncs[(int)easeZoom]);

        controller.AttackCollide(controller.chargedAttack);
        chargedAttackMax = false;
        chargedAttackTime = 0f;
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
    }

    public void Attack(InputAction.CallbackContext ctx)
    {
        if ((controller.hero.State == (int)Entity.EntityState.MOVE || controller.hero.State == (int)Entity.EntityState.ATTACK)
            && !triggerCooldownDash && !weapon.GetComponent<Spear>().IsThrown)
        {
            if (controller.hero.State == (int)Entity.EntityState.ATTACK && !attackQueue)
            {
                attackQueue = true;
            }

            triggerCooldownAttack = true;
            controller.hero.State = (int)Entity.EntityState.ATTACK;
            LaunchedAttack = true;
        }
        else if (controller.hero.State == (int)Entity.EntityState.MOVE && !triggerCooldownDash && weapon.GetComponent<Spear>().IsThrown)
        {
            ThrowOrRetrieveSpear();
        }
    }

    public void Dash(InputAction.CallbackContext ctx)
    {
        if (controller.hero.State == (int)Entity.EntityState.MOVE && !triggerCooldownAttack && !dashCooldown)
        {
            controller.hero.State = (int)Hero.PlayerState.DASH;
            controller.DashDir = controller.LastDir;
            animator.SetTrigger("Dash");
            triggerCooldownDash = true;
            dashCooldown = true;
            controller.VFXWrapper.transform.position = transform.position;
            controller.VFXWrapper.transform.rotation = transform.rotation;
            controller.dashVFX.Play();
        }
    }

    //used as animation event
    public void EndOfSpecialAnimation() //triggers for dash and hit animation to reset state
    {
        controller.hero.State = (int)Entity.EntityState.MOVE;
        attackQueue = false;
        LaunchedAttack = false;
    }

    //used as animation event
    public void EndOfSpecialAnimationAttack() //triggers on attack animations to reset combo
    {
        if (!attackQueue)
        {
            if(!LaunchedChargedAttack)
            {
                controller.hero.State = (int)Entity.EntityState.MOVE;
            }
            controller.ComboCount = 0;
            LaunchedAttack = false;
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
        controller.hero.State = (int)Entity.EntityState.MOVE;
        controller.ComboCount = 0;
        LaunchedAttack = false;
        attackQueue = false;
        LaunchedChargedAttack = false;

        foreach (Collider collider in controller.chargedAttack)
        {
            collider.gameObject.SetActive(false);
        }
    }

    //used as animation event
    public void StartOfAttackAnimation()
    {
        controller.AttackCollide(controller.spearAttacks[controller.ComboCount].data);
        controller.VFXWrapper.transform.position = transform.position;
        controller.VFXWrapper.transform.rotation = transform.rotation;
        controller.spearAttacksVFX[controller.ComboCount].Play();
    }

    public void StartOfIdleAnimation()
    {
        if(!LaunchedChargedAttack)
        {
            controller.hero.State = (int)Entity.EntityState.MOVE;
        }
        controller.ComboCount = 0;
        LaunchedAttack = false;
        attackQueue = false;
    }

    public void ThrowOrRetrieveSpear()
    {
        if(controller.hero.State == (int)Entity.EntityState.MOVE)
        {
            //rotate the player to mouse's direction if playing KB/mouse
            if (DeviceManager.Instance.IsPlayingKB())
            {
                controller.MouseOrientation();
            }
            else
            {
                controller.OrientationErrorMargin(GetComponent<Hero>().Stats.GetValue(Stat.ATK_RANGE));
            }

            if (!LaunchedAttack && !LaunchedChargedAttack)
            {
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
    }
}
