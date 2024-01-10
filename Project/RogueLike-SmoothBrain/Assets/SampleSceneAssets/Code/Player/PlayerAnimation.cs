using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class PlayerAnimation : MonoBehaviour
{
    PlayerController controller;
    public Animator animator;
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
        animator.SetTrigger("BasicAttack");
        controller.hero.State = Hero.PlayerState.ATTACK;
    }

    public void Dash(InputAction.CallbackContext ctx)
    {
        if(controller.hero.State != Hero.PlayerState.DASH)
        {
            animator.SetTrigger("Dash");
            controller.hero.State = Hero.PlayerState.DASH;
            controller.dashDir = controller.Direction;
        }
    }

    public void EndOfSpecialAnimation() //triggers for dash and attack animations to reset currentState
    {
        controller.hero.State = Hero.PlayerState.MOVE;
    }
}
