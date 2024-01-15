using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class PlayerAnimation : MonoBehaviour
{
    PlayerController controller;
    public Animator animator;

    //used to prevent that if you press both dash and attack button to do both at the same time
    float keyCooldown = 0f;
    bool triggerCooldown = false;

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

        if (triggerCooldown)
        {
            keyCooldown += Time.deltaTime;
            if (keyCooldown > 0.2f)
            {
                triggerCooldown = false;
                keyCooldown = 0f;
            }
        }
    }

    public void Attack(InputAction.CallbackContext ctx)
    {
        if ((controller.hero.State == (int)Entity.EntityState.MOVE || controller.hero.State == (int)Entity.EntityState.ATTACK) && !triggerCooldown)
        {
            if (controller.hero.State == (int)Entity.EntityState.ATTACK)
            {
                attackQueue = true;
            }

            animator.SetTrigger("BasicAttack");
            triggerCooldown = true;
            controller.hero.State = (int)Entity.EntityState.ATTACK;
        }
    }

    public void Dash(InputAction.CallbackContext ctx)
    {
        if (controller.hero.State == (int)Entity.EntityState.MOVE && !triggerCooldown)
        {
            controller.hero.State = (int)Hero.PlayerState.DASH;
            controller.dashDir = controller.LastDir;
            animator.SetTrigger("Dash");
            triggerCooldown = true;
        }
    }

    public void EndOfSpecialAnimation() //triggers for dash and hit animation to reset state
    {
        controller.hero.State = (int)Entity.EntityState.MOVE;
        attackQueue = false;
    }

    public void EndOfSpecialAnimationAttack() //triggers on attack animations to reset combo
    {
        if (attackQueue)
        {
            attackQueue = false;
            controller.ComboCount = (++controller.ComboCount) % controller.MAX_COMBO_COUNT;
        }
        else
        {
            controller.hero.State = (int)Entity.EntityState.MOVE;
            controller.ComboCount = 0;
        }
    }
}
