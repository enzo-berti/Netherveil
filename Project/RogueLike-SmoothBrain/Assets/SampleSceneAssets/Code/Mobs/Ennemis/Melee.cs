using UnityEngine;

public class Melee : Sbire
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