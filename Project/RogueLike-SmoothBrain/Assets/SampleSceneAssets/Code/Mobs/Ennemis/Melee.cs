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
        Cervoh();
        
        animator.SetBool("InAttackRange", isInAttackRange);
        animator.SetBool("Triggered", isTriggered);
    }
}
