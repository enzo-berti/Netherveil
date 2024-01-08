using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class PlayerAnimation : MonoBehaviour
{
    PlayerController controller;
    Animator animator;
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
    }

    public void Dash(InputAction.CallbackContext ctx)
    {
        animator.SetTrigger("Dash");
    }
}
