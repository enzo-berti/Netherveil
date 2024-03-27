using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class Knockback : MonoBehaviour
{
    private NavMeshAgent agent;
    private CharacterController characterController;
    private Coroutine knockbackRoutine;
    public Action onObstacleCollide;

    [SerializeField, Range(0.001f, 0.1f)] private float StillThreshold = 0.05f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        characterController = GetComponent<CharacterController>();
    }

    public void GetKnockback(Vector3 direction, float distance, float speed)
    {
        if (knockbackRoutine != null)
            return;

        if(agent != null)
        {
            knockbackRoutine = StartCoroutine(ApplyKnockbackAgent(direction, distance, speed));
        }
        else if (characterController != null)
        {
            knockbackRoutine = StartCoroutine(ApplyKnockbackCharacterController(direction, distance, speed));
        }
       
    }

    private IEnumerator ApplyKnockbackAgent(Vector3 direction, float distance, float speed)
    {
        float timeElapsed = 0f;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = transform.position + direction * distance;

        float duration = distance / speed;
        bool isOnNavMesh = true;

        while (timeElapsed < duration && isOnNavMesh)
        {
            timeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(timeElapsed / duration);

            Vector3 warpPosition = Vector3.Lerp(startPosition, targetPosition, t);

            if (isOnNavMesh = NavMesh.SamplePosition(warpPosition, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
            else
            {
                onObstacleCollide?.Invoke();
            }

            yield return null;
        }

        agent.Warp(targetPosition);
        knockbackRoutine = null;
    }

    protected IEnumerator ApplyKnockbackCharacterController(Vector3 direction, float distance, float speed)
    {
        characterController.enabled = false;

        float timeElapsed = 0f;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = transform.position + direction * distance;

        float duration = distance / speed;
        bool isOnNavMesh = true;

        while (timeElapsed < duration && isOnNavMesh)
        {
            timeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(timeElapsed / duration);

            Vector3 warpPosition = Vector3.Lerp(startPosition, targetPosition, t);

            if (isOnNavMesh = NavMesh.SamplePosition(warpPosition, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
            else
            {
                onObstacleCollide?.Invoke();
            }

            yield return null;
        }

        transform.position = targetPosition;
        characterController.enabled = true;
        knockbackRoutine = null;
    }
}
