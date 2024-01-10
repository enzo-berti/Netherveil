using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kamikaze : Sbire
{
    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        SimpleAI();

        animator.SetBool("InAttackRange", state == EnemyState.ATTACK);
        animator.SetBool("Triggered", state == EnemyState.TRIGGERED || state == EnemyState.ATTACK);
        animator.SetBool("Punch", isAttacking);
    }
}
