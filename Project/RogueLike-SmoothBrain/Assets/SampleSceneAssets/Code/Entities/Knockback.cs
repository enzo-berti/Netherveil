using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class Knockback : MonoBehaviour
{
    private Rigidbody rb;
    private NavMeshAgent agent;
    private Coroutine knockbackRoutine;
    [SerializeField, Range(0.001f, 0.1f)] private float StillThreshold = 0.05f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
    }

    public void GetKnockback(Vector3 force)
    {
        if (knockbackRoutine != null)
            return;

        knockbackRoutine = StartCoroutine(ApplyKnockback(force));
    }

    protected IEnumerator ApplyKnockback(Vector3 force)
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
}
