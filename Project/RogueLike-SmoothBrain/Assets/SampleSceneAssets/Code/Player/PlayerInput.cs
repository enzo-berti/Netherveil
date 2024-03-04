using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(PlayerController))]
public class PlayerInput : MonoBehaviour
{
    IEnumerator chargedAttackCoroutine;
    PlayerInputMap playerInputMap;
    PlayerController controller;
    PlayerInteractions m_interaction;
    Animator animator;
    [SerializeField] GameObject weapon;

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
    readonly float CHARGED_ATTACK_MAX_TIME = 1.5f;
    public float ChargedAttackCoef { get; private set; } = 0f;
    public bool LaunchedChargedAttack { get; private set; } = false;

    void Awake()
    {
        playerInputMap = new PlayerInputMap();
        controller = GetComponent<PlayerController>();
        m_interaction = GetComponent<PlayerInteractions>();
    }

    void Start()
    {
        controller = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        chargedAttackCoroutine = ChargedAttackCoroutine();
    }

    private void OnEnable()
    {
        playerInputMap.Enable();
        playerInputMap.Movement.Movement.performed += controller.ReadDirection;
        playerInputMap.Movement.Movement.canceled += controller.ReadDirection;
        playerInputMap.Attack.Attack.performed += Attack;
        playerInputMap.Dash.Dash.performed += Dash;
        playerInputMap.Interract.Interract.performed += m_interaction.Interract;
        playerInputMap.Attack.Throw.performed += ctx => ThrowSpear();
        playerInputMap.Attack.ChargedAttack.performed += ChargedAttack;
        playerInputMap.Attack.ChargedAttack.canceled += ChargedAttackCanceled;
    }

    private void OnDisable()
    {
        playerInputMap.Disable();
        playerInputMap.Movement.Movement.performed -= controller.ReadDirection;
        playerInputMap.Movement.Movement.canceled -= controller.ReadDirection;
        playerInputMap.Attack.Attack.performed -= Attack;
        playerInputMap.Dash.Dash.performed -= Dash;
        playerInputMap.Interract.Interract.performed -= m_interaction.Interract;
        playerInputMap.Attack.Throw.performed -= ctx => ThrowSpear();
        playerInputMap.Attack.ChargedAttack.performed -= ChargedAttack;
        playerInputMap.Attack.ChargedAttack.canceled -= ChargedAttackCanceled;
    }

    void Update()
    {
        animator.SetFloat("Speed", controller.Direction.magnitude * 10f, 0.1f, Time.deltaTime);

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
            animator.SetTrigger("ChargedAttackCharging");
            triggerCooldownAttack = true;
            controller.hero.State = (int)Entity.EntityState.ATTACK;
            LaunchedChargedAttack = true;

        }
    }

    public void StartChargedAttackCasting()
    {
        controller.ComboCount = 0;
        StartCoroutine(ChargedAttackCoroutine());
    }

    public void ChargedAttackCanceled(InputAction.CallbackContext ctx)
    {
        if(LaunchedChargedAttack)
        {
            StopAllCoroutines();
            animator.ResetTrigger("ChargedAttackRelease");
            animator.SetTrigger("ChargedAttackRelease");
        }
    }

    //used as animation event
    public void ChargedAttackRelease()
    {
        Camera.main.GetComponent<CameraUtilities>().ShakeCamera(0.35f, 0.5f);
        ChargedAttackCoef = chargedAttackMax ? 1 : chargedAttackTime /CHARGED_ATTACK_MAX_TIME;
        controller.AttackCollide(controller.chargedAttack);
        chargedAttackMax = false;
        chargedAttackTime = 0f;
    }

    public IEnumerator ChargedAttackCoroutine()
    {
        while (chargedAttackTime < CHARGED_ATTACK_MAX_TIME)
        {
            chargedAttackTime += Time.deltaTime;
            yield return null;
        }

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
                controller.ComboCount = (++controller.ComboCount) % controller.MAX_COMBO_COUNT;
            }

            animator.SetTrigger("BasicAttack");
            triggerCooldownAttack = true;
            controller.hero.State = (int)Entity.EntityState.ATTACK;
            LaunchedAttack = true;
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
            controller.hero.State = (int)Entity.EntityState.MOVE;
            controller.ComboCount = 0;
            LaunchedAttack = false;
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
    }

    public void ThrowSpear()
    {
        //rotate the player to mouse's direction if playing KB/mouse
        if (InputDeviceManager.Instance.IsPlayingKB())
        {
            controller.MouseOrientation();
        }
        else
        {
            controller.OrientationErrorMargin(GetComponent<Hero>().Stats.GetValueStat(Stat.ATK_RANGE));
        }

        if (!GetComponent<PlayerInput>().LaunchedAttack)
        {
            Spear spear = weapon.GetComponent<Spear>();

            // If spear is being thrown we can't recall this attack
            if (spear.IsThrowing) return;
            if (!spear.IsThrown)
            {
                spear.Throw(this.transform.position + this.transform.forward *controller.hero.Stats.GetValueStat(Stat.ATK_RANGE));
            }
            else
            {
                spear.Return();
            }
        }
    }
}
