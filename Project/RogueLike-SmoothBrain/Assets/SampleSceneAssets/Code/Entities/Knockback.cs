using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Knockback : MonoBehaviour
{
    private NavMeshAgent agent;
    private CharacterController characterController;
    private Hero hero;
    private Animator animator;
    private Coroutine knockbackRoutine;

    [SerializeField] private float distanceFactor = 1f;

    /// <summary>
    /// int _value, bool isCrit = false, bool notEffectDamages = true
    /// </summary>
    public Action<int, bool, bool> onObstacleCollide;
    [SerializeField] private int damageTakeOnObstacleCollide = 10;

    //[SerializeField, Range(0.001f, 0.1f)] private float StillThreshold = 0.05f; // Commenter par Dorian -> WARNING

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        characterController = GetComponent<CharacterController>();
        hero = GetComponent<Hero>();
        animator = GetComponentInChildren<Animator>();
    }

    public void GetKnockback(Vector3 direction, float distance, float speed)
    {
        if (knockbackRoutine != null)
            return;

        if (agent != null)
        {
            knockbackRoutine = StartCoroutine(ApplyKnockbackAgent(direction, distance, speed));
        }
        else if (characterController != null)
        {
            animator.SetBool("IsKnockback", true);
            hero.State = (int)Hero.PlayerState.KNOCKBACK;
            knockbackRoutine = StartCoroutine(ApplyKnockbackPlayer(direction, distance, speed));
        }

    }

    private IEnumerator ApplyKnockbackAgent(Vector3 direction, float distance, float speed)
    {
        float timeElapsed = 0f;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = transform.position + direction * distance * distanceFactor;

        float duration = distance * distanceFactor / speed;
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
                onObstacleCollide?.Invoke(damageTakeOnObstacleCollide, false, true);
            }

            yield return null;
        }

        knockbackRoutine = null;
    }

    protected IEnumerator ApplyKnockbackPlayer(Vector3 direction, float distance, float speed)
    {
        characterController.enabled = false;

        float timeElapsed = 0f;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = transform.position + direction * distance * distanceFactor;

        float duration = distance * distanceFactor / speed;
        bool hitObstacle = false;

        while (timeElapsed < duration && !hitObstacle)
        {
            timeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(timeElapsed / duration);

            Vector3 lastPos = transform.position;
            Vector3 nextPos = Vector3.Lerp(startPosition, targetPosition, t);

            if (hitObstacle = Physics.Raycast(transform.position, direction, Vector3.Distance(lastPos, nextPos), ~LayerMask.GetMask("Entity")))
            {
                onObstacleCollide?.Invoke(damageTakeOnObstacleCollide, false, true);
            }
            else
            {
                transform.position = nextPos;
            }

            yield return null;
        }

        characterController.enabled = true;
        hero.State = (int)Entity.EntityState.MOVE;
        animator.SetBool("IsKnockback", false);
        knockbackRoutine = null;
    }
}

