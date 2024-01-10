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
        //SimpleAI();
        
        //animator.SetBool("InAttackRange", state == EnemyState.ATTACK);
        //animator.SetBool("Triggered", state == EnemyState.TRIGGERED || state == EnemyState.ATTACK);
        //animator.SetBool("Punch", isAttacking);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger");
        Debug.Log(other.name);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collision melee");
        Debug.Log(collision.gameObject.name);
    }
}
