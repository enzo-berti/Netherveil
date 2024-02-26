using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(PlayerController))]
public class PlayerInput : MonoBehaviour
{

    public Plane planeOfDoom;
    IEnumerator chargedAttackCoroutine;
    PlayerInputMap playerInputMap;
    PlayerController controller;
    PlayerInteractions m_interaction;
    Animator animator;
    [SerializeField] GameObject weapon;

    //used for the error margin for attacks to auto-redirect on enemies in vision cone
    const float VISION_CONE_ANGLE = 45f;
    const float VISION_CONE_RANGE = 8f;

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
    bool LaunchedSpecialAttack = false;

    void Awake()
    {
        playerInputMap = new PlayerInputMap();
        controller = GetComponent<PlayerController>();
        m_interaction = GetComponent<PlayerInteractions>();
        planeOfDoom = new Plane(Vector3.up, 0f);
        planeOfDoom.SetNormalAndPosition(Vector3.up, new Vector3(0f, 0.05f, 0f));
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
            controller.ComboCount = 0;
            animator.SetTrigger("ChargedAttackCharging");
            triggerCooldownAttack = true;
            controller.hero.State = (int)Entity.EntityState.ATTACK;
            LaunchedSpecialAttack = true;
            StartCoroutine(chargedAttackCoroutine);
        }
    }

    public void ChargedAttackCanceled(InputAction.CallbackContext ctx)
    {
        StopCoroutine(chargedAttackCoroutine);
        if(LaunchedSpecialAttack)
        {
            animator.ResetTrigger("ChargedAttackRelease");
            animator.SetTrigger("ChargedAttackRelease");
        }
    }

    public void ChargedAttackRelease()
    {
        //add check for damage if max
        AttackCollide(controller.chargedAttack);
        chargedAttackMax = false;
    }

    public IEnumerator ChargedAttackCoroutine()
    {
        while (chargedAttackTime < 0.5f)
        {
            chargedAttackTime += Time.deltaTime;
            yield return null;
        }

        chargedAttackTime = 0f;
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

    public void EndOfSpecialAnimation() //triggers for dash and hit animation to reset state
    {
        controller.hero.State = (int)Entity.EntityState.MOVE;
        attackQueue = false;
        LaunchedAttack = false;
    }

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
        LaunchedSpecialAttack = false;

        foreach (Collider collider in controller.chargedAttack)
        {
            collider.gameObject.SetActive(false);
        }
    }

    public void StartOfAttackAnimation()
    {
        AttackCollide(controller.spearAttacks[controller.ComboCount].data);
    }

    void AttackCollide(List<Collider> colliders)
    {
        foreach (Collider collider in colliders)
        {
            collider.gameObject.SetActive(true);
        }

        //rotate the player to mouse's direction if playing KB/mouse
        if (InputDeviceManager.Instance.IsPlayingKB())
        {
            MouseOrientation();
        }
        OrientationErrorMargin();

        //used so that it isn't cast from his feet to ensure that there is no ray fail by colliding with spear or ground
        Vector3 rayOffset = Vector3.up;


        foreach (Collider spearCollider in colliders)
        {
            Collider[] tab = controller.CheckAttackCollide(spearCollider, transform.position + rayOffset, "Enemy", -1, QueryTriggerInteraction.UseGlobal, LayerMask.GetMask("Map"));
            if (tab.Length > 0)
            {
                foreach (Collider col in tab)
                {
                    if (col.gameObject.GetComponent<IDamageable>() != null)
                    {
                        //Debug.Log(col.gameObject.name);
                        controller.hero.Attack(col.gameObject.GetComponent<IDamageable>());
                    }
                }
            }
        }
    }

    void MouseOrientation()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (planeOfDoom.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            float angle = transform.AngleOffsetToFaceTarget(new Vector3(hitPoint.x, this.transform.position.y, hitPoint.z));
            if (angle != float.MaxValue)
            {
                Vector3 a = transform.eulerAngles;
                a.y += angle;
                transform.eulerAngles = a;
                GetComponent<PlayerController>().CurrentTargetAngle = transform.eulerAngles.y;
            }
        }
    }

    void OrientationErrorMargin(float visionConeRange = VISION_CONE_RANGE)
    {
        Transform targetTransform = PhysicsExtensions.OverlapVisionCone(transform.position, VISION_CONE_ANGLE, visionConeRange, transform.forward, LayerMask.GetMask("Entity"))
        .Select(x => x.GetComponent<Transform>())
        .OrderBy(x => Vector3.Distance(x.transform.position, transform.position))
        .FirstOrDefault();

        if (targetTransform != null)
        {
            float angle = transform.AngleOffsetToFaceTarget(targetTransform.position, VISION_CONE_ANGLE);
            if (angle != float.MaxValue)
            {
                Vector3 a = transform.eulerAngles;
                a.y += angle;
                transform.eulerAngles = a;
                GetComponent<PlayerController>().CurrentTargetAngle = transform.eulerAngles.y;
            }
        }
    }

    public void ThrowSpear()
    {
        //rotate the player to mouse's direction if playing KB/mouse
        if (InputDeviceManager.Instance.IsPlayingKB())
        {
            MouseOrientation();
        }
        else
        {
            OrientationErrorMargin(GetComponent<Hero>().Stats.GetValueStat(Stat.ATK_RANGE));
        }

        if (!GetComponent<PlayerInput>().LaunchedAttack)
        {
            Spear spear = weapon.GetComponent<Spear>();

            // If spear is being thrown we can't recall this attack
            if (spear.IsThrowing) return;
            if (!spear.IsThrown)
            {
                spear.Throw(this.transform.position + this.transform.forward * this.gameObject.GetComponent<Hero>().Stats.GetValueStat(Stat.ATK_RANGE));
            }
            else
            {
                spear.Return();
            }
        }
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        ////Collider[] collide = PhysicsExtensions.OverlapVisionCone(transform.position, VISION_CONE_ANGLE, VISION_CONE_RANGE, transform.forward, LayerMask.GetMask("Entity"));

        //Handles.color = new Color(1, 0, 0, 0.25f);
        ////if (collide.Length != 0)
        ////{
        ////    Handles.color = new Color(0, 1, 0, 0.25f);
        ////}

        //Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, VISION_CONE_ANGLE / 2f, VISION_CONE_RANGE);
        //Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -VISION_CONE_ANGLE / 2f, VISION_CONE_RANGE);
    }
#endif
}
