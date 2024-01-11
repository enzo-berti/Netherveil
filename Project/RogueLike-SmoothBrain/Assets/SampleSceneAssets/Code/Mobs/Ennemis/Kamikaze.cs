using UnityEngine;

public class Kamikaze : Sbire
{
    Animator animator;

    new private void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
    }

    new void Update()
    {
        base.Update();
        SimpleAI();

        animator.SetBool("InAttackRange", state == EnemyState.ATTACK);
        animator.SetBool("Triggered", state == EnemyState.TRIGGERED || state == EnemyState.ATTACK);
        animator.SetBool("Punch", isAttacking);
    }
}
