using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class Knockback : MonoBehaviour
{
    private Rigidbody rb;
    private NavMeshAgent agent;
    private CharacterController characterController;
    private Coroutine knockbackRoutine;
    private Animator animator;
    private Hero hero;
    private Collider col;
    [SerializeField, Range(0.001f, 0.1f)] private float StillThreshold = 0.05f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        hero = GetComponent<Hero>();
        col = GetComponent<CapsuleCollider>();
    }

    public void GetKnockback(Vector3 force)
    {
        if (knockbackRoutine != null)
            return;

        if(agent != null)
        {
            knockbackRoutine = StartCoroutine(ApplyKnockbackAgent(force));
        }
        else if (characterController != null)
        {
            knockbackRoutine = StartCoroutine(ApplyKnockbackCharacterController(force));
        }
       
    }

    protected IEnumerator ApplyKnockbackAgent(Vector3 force)
    {
        yield return null;
        agent.enabled = false;
        rb.isKinematic = false;
        rb.AddForce(force, ForceMode.Impulse);

        yield return new WaitForFixedUpdate();
        yield return new WaitUntil(() => rb.velocity.magnitude < StillThreshold);
        yield return new WaitForSeconds(0.25f);

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        agent.Warp(transform.position);
        agent.enabled = true;

        knockbackRoutine = null;
    }

    protected IEnumerator ApplyKnockbackCharacterController(Vector3 force)
    {
        yield return null;
        if(characterController != null) 
        {
            characterController.enabled = false;
            col.enabled = true;
            animator.SetBool("IsKnockback", true);
            hero.State = (int)Hero.PlayerState.KNOCKBACK;
            rb.isKinematic = false;
            rb.AddForce(force, ForceMode.Impulse);

            yield return new WaitForFixedUpdate();
            yield return new WaitUntil(() => rb.velocity.magnitude < StillThreshold);

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;

            animator.SetBool("IsKnockback", false);
            characterController.enabled = true;
            col.enabled = false;
            hero.State = (int)Entity.EntityState.MOVE;

            knockbackRoutine = null;
        }
    }
}
