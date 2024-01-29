using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class PlayerInput : MonoBehaviour
{


    PlayerInputMap playerInputMap;
    PlayerController controller;
    PlayerInteractions m_interaction;
    PlayerAttack m_attack;
    Animator animator;

    //used for the error margin for attacks to auto-redirect on enemies in vision cone
    readonly float VISION_CONE_ANGLE = 45f;
    readonly float VISION_CONE_RANGE = 8f;

    //used to prevent that if you press both dash and attack button to do both at the same time
    float keyCooldown = 0f;
    bool triggerCooldownAttack = false;
    bool triggerCooldownDash = false;

    bool attackQueue = false;

    void Awake()
    {
        playerInputMap = new PlayerInputMap();
        controller = GetComponent<PlayerController>();
        m_interaction = GetComponent<PlayerInteractions>();
        m_attack = GetComponent<PlayerAttack>();
    }

    void Start()
    {
        controller = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        playerInputMap.Enable();
        playerInputMap.Movement.Movement.performed += controller.ReadDirection;
        playerInputMap.Movement.Movement.canceled += controller.ReadDirection;
        playerInputMap.Attack.Attack.performed += Attack;
        playerInputMap.Dash.Dash.performed += Dash;
        playerInputMap.Interract.Interract.performed += m_interaction.Interract;
        playerInputMap.Attack.Throw.performed += ctx => m_attack.ThrowSpear();
    }

    private void OnDisable()
    {
        playerInputMap.Disable();
        playerInputMap.Movement.Movement.performed -= controller.ReadDirection;
        playerInputMap.Movement.Movement.canceled -= controller.ReadDirection;
        playerInputMap.Attack.Attack.performed -= Attack;
        playerInputMap.Dash.Dash.performed -= Dash;
        playerInputMap.Interract.Interract.performed -= m_interaction.Interract;
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
    }

    public void Attack(InputAction.CallbackContext ctx)
    {
        if ((controller.hero.State == (int)Entity.EntityState.MOVE || controller.hero.State == (int)Entity.EntityState.ATTACK) && !triggerCooldownDash)
        {
            if (controller.hero.State == (int)Entity.EntityState.ATTACK && !attackQueue)
            {
                attackQueue = true;
                controller.ComboCount = (++controller.ComboCount) % controller.MAX_COMBO_COUNT;
            }

            animator.SetTrigger("BasicAttack");
            triggerCooldownAttack = true;
            controller.hero.State = (int)Entity.EntityState.ATTACK;
        }
    }

    public void Dash(InputAction.CallbackContext ctx)
    {
        if (controller.hero.State == (int)Entity.EntityState.MOVE && !triggerCooldownAttack)
        {
            controller.hero.State = (int)Hero.PlayerState.DASH;
            controller.DashDir = controller.LastDir;
            animator.SetTrigger("Dash");
            triggerCooldownDash = true;
        }
    }

    public void EndOfSpecialAnimation() //triggers for dash and hit animation to reset state
    {
        controller.hero.State = (int)Entity.EntityState.MOVE;
        attackQueue = false;
    }

    public void EndOfSpecialAnimationAttack() //triggers on attack animations to reset combo
    {
        if (!attackQueue)
        {
            controller.hero.State = (int)Entity.EntityState.MOVE;
            controller.ComboCount = 0;
            attackQueue = false;
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

    public void StartOfAttackAnimation()
    {
        foreach (Collider spearCollider in controller.spearAttacks[controller.ComboCount].data)
        {
            spearCollider.gameObject.SetActive(true);
        }

        foreach (Collider spearCollider in controller.spearAttacks[controller.ComboCount].data)
        {
            Collider[] tab = controller.CheckAttackCollide(spearCollider, LayerMask.GetMask("Entity"));

            foreach (Collider col in tab)
            {
                if (col.gameObject.GetComponent<IDamageable>() != null)
                {
                    controller.hero.Attack(col.gameObject.GetComponent<IDamageable>());
                }
            }
        }

        //rotate the player to mouse's direction if playing KB/mouse
        if(Gamepad.all.Count == 0)
        {
            MouseOrientation();
        }
        OrientationErrorMargin();
    }

    void MouseOrientation()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit))
        {
            float angle = transform.IsTargetLeftOrRightSide(new Vector3(hit.point.x, this.transform.position.y, hit.point.z), VISION_CONE_ANGLE);
            if (angle != float.MaxValue)
            {
                GetComponent<PlayerController>().CurrentTargetAngle += angle;
            }
        }
    }

    void OrientationErrorMargin()
    {
        Transform targetTransform = PhysicsExtensions.OverlapVisionCone(transform.position, VISION_CONE_ANGLE, VISION_CONE_RANGE, transform.forward, LayerMask.GetMask("Entity"))
        .Select(x => x.GetComponent<Transform>())
        .FirstOrDefault();

        if (targetTransform != null)
        {
            float angle = transform.IsTargetLeftOrRightSide(targetTransform.position, VISION_CONE_ANGLE);
            if(angle != float.MaxValue)
            {
                GetComponent<PlayerController>().CurrentTargetAngle += angle;
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        //Collider[] collide = PhysicsExtensions.OverlapVisionCone(transform.position, VISION_CONE_ANGLE, VISION_CONE_RANGE, transform.forward, LayerMask.GetMask("Entity"));

        Handles.color = new Color(1, 0, 0, 0.25f);
        //if (collide.Length != 0)
        //{
        //    Handles.color = new Color(0, 1, 0, 0.25f);
        //}

        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, VISION_CONE_ANGLE / 2f, VISION_CONE_RANGE);
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -VISION_CONE_ANGLE / 2f, VISION_CONE_RANGE);
    }
#endif
}
