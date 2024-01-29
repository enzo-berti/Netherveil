using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class PlayerAnimation : MonoBehaviour
{
    PlayerController controller;
    [HideInInspector] public Animator animator;

    //used to prevent that if you press both dash and attack button to do both at the same time
    float keyCooldown = 0f;
    bool triggerCooldownAttack = false;
    bool triggerCooldownDash = false;

    bool attackQueue = false;


    void Start()
    {
        controller = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("Speed", controller.Direction.magnitude, 0.1f, Time.deltaTime);

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

        Transform targetTransform = GetComponent<VisionCone>().GetTarget("Enemy");
        if (targetTransform != null)
        {
            Vector3 playerToTargetVec = targetTransform.position - transform.position;
            float angle = Vector3.Angle(playerToTargetVec, transform.forward);
            if (angle <= GetComponent<VisionCone>().angle && angle > float.Epsilon)
            {
                //vector that describes the enemy's position offset from the player's position along the player's left/right, up/down, and forward/back axes
                Vector3 enemyDirectionLocal = transform.InverseTransformPoint(targetTransform.position);

                //Left side of player
                if (enemyDirectionLocal.x < 0)
                {
                    GetComponent<PlayerController>().CurrentTargetAngle -= angle;
                }
                //Right side of player
                else if (enemyDirectionLocal.x > 0)
                {
                    GetComponent<PlayerController>().CurrentTargetAngle += angle;
                }
            }
        }
    }
}
