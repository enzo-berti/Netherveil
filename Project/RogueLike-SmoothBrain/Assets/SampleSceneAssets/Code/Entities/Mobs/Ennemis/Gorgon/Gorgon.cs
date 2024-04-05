using System.Collections;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.AI;
using System;
using FMODUnity;
using UnityEngine.VFX;
using System.Xml.Linq;
using UnityEngine.VFX.Utility;
#if UNITY_EDITOR
using UnityEditor;
#endif

// Hate this mob
public class Gorgon : Mobs, IGorgon
{
    private enum RangeState
    {
        WANDERING,
        TRIGGERED,
        STAGGERED,
        CHARGING
    }

    private IAttacker.HitDelegate onHit;
    private IAttacker.AttackDelegate onAttack;
    public IAttacker.HitDelegate OnHit { get => onHit; set => onHit = value; }
    public IAttacker.AttackDelegate OnAttack { get => onAttack; set => onAttack = value; }

    [SerializeField] private float timeBetweenAttack;
    [SerializeField] private float timeBetweenFleeing;
    [SerializeField] private GameObject pfBomb;
    [SerializeField] private Transform hand;
    [SerializeField] private EventReference hitSFX;
    [SerializeField] private EventReference deathSFX;
    public VisualEffect dashVFX;
    [Header("Range Parameters")]
    [SerializeField, Min(0)] private float staggerDuration;

    private bool canAttack = true;
    private bool canFlee = true;
    private bool isFleeing = false;
    private bool isGoingOnPlayer = false;
    private bool isAttacking = false;
    private bool isSmoothCoroutineOn = false;
    private bool hasLaunchAnim = false;
    private bool hasRemoveHead = false;
    public bool HasLaunchAnim { get => hasLaunchAnim; set => hasLaunchAnim = value; }
    public bool HasRemoveHead { get => hasRemoveHead; set => hasRemoveHead = value; }
    private float DistanceToFlee
    {
        get => stats.GetValue(Stat.ATK_RANGE) / 1.5f;
    }

    Animator animator;

    Transform playerTransform;
    public List<Status> StatusToApply { get => statusToApply; }
    protected override void Start()
    {
        base.Start();
        // getter(s) reference
        animator = GetComponentInChildren<Animator>();
        playerTransform = GameObject.FindWithTag("Player").transform;
    }

    protected override IEnumerator EntityDetection()
    {
        while (true)
        {
            if (!agent.enabled)
            {
                yield return null;
                continue;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    protected override IEnumerator Brain()
    {
        while (true)
        {
            yield return null;

            if (!agent.enabled)
                continue;

            float distanceFromPlayer = Vector3.Distance(transform.position, playerTransform.position);

            // if he can attack and he's at range then attack
            if (!isGoingOnPlayer && !isFleeing && canAttack && distanceFromPlayer <= stats.GetValue(Stat.ATK_RANGE))
            {
                if (agent.hasPath)
                {
                    agent.ResetPath();
                }
                // Il se tourne vers le player pour l'attaquer
                this.transform.LookAt(playerTransform.transform.position);

                // Quand il est tourné vers le player il lui envoie une bombe
                StartCoroutine(LongRangeAttack());
            }

            // If he can attack but he's not at range, dash to the player
            else if (!isGoingOnPlayer && !isFleeing && canAttack && !isAttacking)
            {
                isGoingOnPlayer = true;

                // Take a random point around the player pos in 2D then convert it in 3D
                Vector2 pointToReach2D = MathsExtension.GetPointOnCircle(new Vector2(playerTransform.position.x, playerTransform.position.z), 7);
                Vector3 pointToReach3D = new(pointToReach2D.x, this.transform.position.y, pointToReach2D.y);

                // Replace the point on navMesh
                NavMesh.SamplePosition(pointToReach3D, out NavMeshHit hit, float.PositiveInfinity, NavMesh.AllAreas);
                pointToReach3D = hit.position;

                List<Vector3> listDashes = GetDashesPath(pointToReach3D, 3);
                StartCoroutine(DashToPos(listDashes));
            }

            // Flee if he can't attack
            else if (canFlee && !isGoingOnPlayer && !isFleeing && !isAttacking && distanceFromPlayer <= DistanceToFlee)
            {
                // TODO : Change this bad behaviour

                Vector2 playerPosXZ = new(playerTransform.position.x, playerTransform.position.z);

                Vector3 direction = this.transform.position - playerTransform.position;
                Vector2 Point2DToReach = MathsExtension.GetPointOnCone(playerPosXZ, direction, 10, 90);
                Vector3 point3DToReach = new(Point2DToReach.x, playerTransform.position.y, Point2DToReach.y);
                //this.transform.forward = direction;
                MoveTo(point3DToReach);
                isFleeing = true;
                StartCoroutine(WaitToFleeAgain(timeBetweenFleeing));
            }

            else if (!agent.hasPath)
            {
                if (isFleeing)
                {
                    isFleeing = false;
                    Vector3 direction = playerTransform.position - this.transform.position;
                    this.transform.forward = direction;
                }
            }

            //UpdateStates();
        }
    }

    private IEnumerator DashToPos(List<Vector3> listDashes)
    {
        for (int i = 1; i < listDashes.Count; i++)
        {
            StartCoroutine(GoSmoothToPosition(listDashes[i]));
            animator.ResetTrigger("Dash");
            animator.SetTrigger("Dash");
            yield return new WaitUntil(() => isSmoothCoroutineOn == false);
        }
        isGoingOnPlayer = false;
    }

    private IEnumerator GoSmoothToPosition(Vector3 posToReach)
    {
        isSmoothCoroutineOn = true;

        float timer = 0;
        Vector3 basePos = this.transform.position;
        Vector3 newPos;
        // Face to his next direction
        this.transform.forward = posToReach - basePos;
        while (timer < 1f)
        {
            newPos = Vector3.Lerp(basePos, posToReach, timer);
            agent.Warp(newPos);
            timer += Time.deltaTime * 5;
            timer = timer > 1 ? 1 : timer;
            yield return null;
        }
        yield return new WaitForSeconds(0.25f);
        isSmoothCoroutineOn = false;
    }

    private IEnumerator LongRangeAttack()
    {
        // Stop bomber's attack
        canAttack = false;
        isAttacking = true;
        // Wait to face player

        animator.SetTrigger("Attack");
        float timeToThrow = 0.8f;
        Vector2 pointToReach2D = MathsExtension.GetPointOnCircle(new Vector2(playerTransform.position.x, playerTransform.position.z), 1f);
        Vector3 pointToReach3D = new(pointToReach2D.x, playerTransform.position.y, pointToReach2D.y);
        if (NavMesh.SamplePosition(pointToReach3D, out var hit, 3, -1))
        {
            pointToReach3D = hit.position;
        }

        yield return new WaitWhile(() => HasRemoveHead == false);
        if (this.gameObject != null)
        {
            GameObject bomb = Instantiate(pfBomb, hand);
            yield return new WaitWhile(() => hasLaunchAnim == false);
            bomb.transform.rotation = Quaternion.identity;
            bomb.transform.parent = null;

            ExplodingBomb exploBomb = bomb.GetComponent<ExplodingBomb>();

            exploBomb.ThrowToPos(this, pointToReach3D, timeToThrow);
            exploBomb.SetTimeToExplode(timeToThrow * 1.5f);
            exploBomb.Activate();

            yield return new WaitForSeconds(0.5f);
            isAttacking = false;
            yield return new WaitForSeconds(timeBetweenAttack);
            canAttack = true;
        }

    }



    public void ApplyDamage(int _value, IAttacker attacker, bool hasAnimation = true)
    {
        if (stats.GetValue(Stat.HP) <= 0)
            return;

        Stats.DecreaseValue(Stat.HP, _value, false);
        lifeBar.ValueChanged(stats.GetValue(Stat.HP));

        if (hasAnimation)
        {
            FloatingTextGenerator.CreateDamageText(_value, transform.position);
            AudioManager.Instance.PlaySound(hitSFX, transform.position);
            StartCoroutine(HitRoutine());
        }

        if (stats.GetValue(Stat.HP) <= 0)
        {
            Death();
        }
    }

    /// <summary>
    /// Get a random path with multiple dashes to reach a point
    /// </summary>
    /// <param name="posToReach"></param>
    /// <param name="nbDash"></param>
    /// <returns></returns>
    public List<Vector3> GetDashesPath(Vector3 posToReach, int nbDash)
    {
        List<Vector3> path = new()
        {
            this.transform.position
        };

        
        NavMeshPath navPath = new();
        NavMesh.CalculatePath(this.transform.position, posToReach, -1, navPath);
        // First corner is initPos and last is endPos
        for (int i = 1; i < navPath.corners.Length - 1; i++)
        {
            path.Add(navPath.corners[i]);
        }
        float distance = Vector3.Distance(transform.position, posToReach);
        // If it's a straight line
        if (path.Count == 1)
        {
            for (int i = 1; i < nbDash; i++)
            {
                // We avoid y value because we only move in x and z
                Vector2 posToReach2D = new(posToReach.x, posToReach.z);

                // Virtually get the "current" position of the dasher ( get the position he reached after his previous dash )
                Vector2 curPos2D = new(path[i - 1].x, path[i - 1].z);

                Vector2 direction = posToReach2D - curPos2D;

                Vector2 posOnCone = this.transform.position;

                if (direction != Vector2.zero)
                    posOnCone = MathsExtension.GetPointOnCone(curPos2D, direction, distance / nbDash, 60);

                Vector3 posOnCone3D = new(posOnCone.x, this.transform.position.y, posOnCone.y);
                if (NavMesh.SamplePosition(posOnCone3D, out var hit, 10, NavMesh.AllAreas))
                {
                    posOnCone3D = hit.position;
                    path.Add(posOnCone3D);
                }


            }
            //path.Add(posToReach);
        }

        // We finally add the position that we want to reach after every dash
        path.Add(posToReach);
        return path;
    }
    
    public void Death()
    {
        AudioManager.Instance.PlaySound(deathSFX, this.transform.position);
        OnDeath?.Invoke(transform.position);
        Destroy(transform.parent.gameObject);
        GameObject.FindWithTag("Player").GetComponent<Hero>().OnKill?.Invoke(this);
    }

    public void Attack(IDamageable damageable)
    {
        int damages = (int)stats.GetValue(Stat.ATK);
        onHit?.Invoke(damageable, this);
        damageable.ApplyDamage(damages, this);
    }

    public void MoveTo(Vector3 posToMove)
    {
        agent.SetDestination(posToMove);
    }

    public IEnumerator WaitToFleeAgain(float delay)
    {
        canFlee = false;
        yield return new WaitForSeconds(delay);
        canFlee = true;
    }

#if UNITY_EDITOR
    //private void DisplayFleeingRange(float _angle)
    //{
    //    Handles.color = new Color(1, 1, 0f, 0.25f);
    //    Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, _angle / 2f, FleeingRange);
    //    Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -_angle / 2f, FleeingRange);

    //    Handles.color = Color.white;
    //    Handles.DrawWireDisc(transform.position, Vector3.up, FleeingRange);
    //}

    //private void OnDrawGizmos()
    //{
    //    if (!Selection.Contains(gameObject))
    //        return;

    //    DisplayVisionRange(isFighting ? 360 : (int)stats.GetValue(Stat.CATCH_RADIUS));
    //    DisplayAttackRange(isFighting ? 360 : (int)stats.GetValue(Stat.CATCH_RADIUS));
    //    DisplayFleeingRange(isFighting ? 360 : (int)stats.GetValue(Stat.CATCH_RADIUS));
    //    DisplayInfos();
    //}
#endif
}

// quand le joueur est trop près, le range va se réfugier derrière le tank
// quand le joueur est trop près, si aucun tank n'est à proximité il va fuir en ligne droite
// il ne le fera pas en boucle, il aura un cd sur sa fuite
// lorsqu'il est en cd, il va attaquer le joueur simplement -> TODO