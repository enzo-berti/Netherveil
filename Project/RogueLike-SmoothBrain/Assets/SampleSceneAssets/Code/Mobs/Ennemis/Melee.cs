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
        
        animator.SetBool("InAttackRange", state == EntityState.ATTACK);
        animator.SetBool("Triggered", state == EntityState.TRIGGERED || state == EntityState.ATTACK);
        animator.SetBool("Punch", isAttacking);
    }
}
