using Unity.VisualScripting;
using UnityEngine;

public class Melee : Sbire
{
    Animator animator;

    new private void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
    }

    protected override void Update()
    {
        base.Update();

        animator.SetBool("InAttackRange", State == (int)EntityState.ATTACK);
        animator.SetBool("Triggered", State == (int)EnemyState.TRIGGERED || State == (int)EntityState.ATTACK || agent.hasPath);
        animator.SetBool("Punch", isAttacking);
    }
}