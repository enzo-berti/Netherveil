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

        animator.SetBool("InAttackRange", State == (int)EnemyState.ATTACK);
        animator.SetBool("Triggered", State == (int)EnemyState.TRIGGERED || State == (int)EnemyState.ATTACK);
        animator.SetBool("Punch", isAttacking);
    }
}
