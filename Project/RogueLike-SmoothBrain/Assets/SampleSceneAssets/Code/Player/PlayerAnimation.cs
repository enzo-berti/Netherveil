using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class PlayerAnimation : MonoBehaviour
{
    PlayerController controller;
    public Animator animator;
    bool hasTriggeredAttack = false;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("Speed", controller.Direction.magnitude, 0.1f, Time.deltaTime);
    }

    public void Attack(InputAction.CallbackContext ctx)
    {
        if (controller.hero.State == Hero.PlayerState.MOVE || controller.hero.State == Hero.PlayerState.ATTACK)
        {
            controller.hero.State = Hero.PlayerState.ATTACK;
            animator.SetTrigger("BasicAttack");
            hasTriggeredAttack = true;
            controller.ComboCount = (++controller.ComboCount) % controller.MAX_COMBO_COUNT;
            animator.SetInteger("ComboCount", controller.ComboCount);
        }
    }

    public void Dash(InputAction.CallbackContext ctx)
    {
        if(controller.hero.State == Hero.PlayerState.MOVE)
        {
            controller.hero.State = Hero.PlayerState.DASH;
            controller.dashDir = controller.LastDir;
            animator.SetTrigger("Dash");
        }
    }

    public void EndOfSpecialAnimation() //triggers for dash and attack animations to reset currentState
    {
        controller.hero.State = Hero.PlayerState.MOVE;
        hasTriggeredAttack = false;
    }

    public void EndOfSpecialAnimationAttack() //triggers for dash and attack animations to reset currentState
    {
        controller.hero.State = Hero.PlayerState.MOVE;
        if(hasTriggeredAttack)
        {
            hasTriggeredAttack = false;
            controller.ComboCount = -1;
            animator.SetInteger("ComboCount", controller.ComboCount);
        }
    }
}
