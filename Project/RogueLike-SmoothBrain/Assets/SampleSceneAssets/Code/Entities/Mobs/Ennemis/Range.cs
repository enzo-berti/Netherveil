using System.Collections;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.AI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Range : Mobs, IRange
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
    [Header("Range Parameters")]
    [SerializeField, Min(0)] private float staggerDuration;

    private bool canAttack = true;
    private bool canFlee = true;
    private bool isFleeing = false;
    private bool isGoingOnPlayer = false;
    private bool isAttacking = false;
    private bool isSmoothCoroutineOn = false;
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

            // S'il a atteint sa position et qu'il est toujours à range pour attaquer il va le faire
            if (!isGoingOnPlayer && !isFleeing && canAttack && distanceFromPlayer <= stats.GetValue(Stat.ATK_RANGE))
            {
                if (agent.hasPath)
                {
                    Debug.Log("ResetPath");
                    agent.ResetPath();
                }
                // Il se tourne vers le player pour l'attaquer
                //StartCoroutine(FaceToPlayer(player.transform));
                this.transform.LookAt(playerTransform.transform.position);
                // Quand il est tourné vers le player il lui envoie une bombe
                StartCoroutine(LongRangeAttack(playerTransform.transform));
            }
            // Si l'ennemi peut attaquer il va se deplacer à range du player
            else if (!isGoingOnPlayer && !isFleeing && canAttack && !isAttacking)
            {
                isGoingOnPlayer = true;
                Vector2 pointToReach2D = GetPointOnCircle(new Vector2(playerTransform.position.x, playerTransform.position.z), 7);
                Vector3 pointToReach3D = new(pointToReach2D.x, this.transform.position.y, pointToReach2D.y);
                NavMesh.SamplePosition(pointToReach3D, out NavMeshHit hit, float.PositiveInfinity, NavMesh.AllAreas);
                pointToReach3D = hit.position;
                List<Vector3> listDashes = GetDashesPath(pointToReach3D, 4);
                StartCoroutine(DashToPos(listDashes));
            }
            // Flee
            else if (canFlee && !isGoingOnPlayer && !isFleeing && !isAttacking && distanceFromPlayer <= DistanceToFlee)
            {
                Vector2 playerPosXZ = new(playerTransform.position.x, playerTransform.position.z);
                Vector2 Point2DToReach = GetPointOnCircle(playerPosXZ, stats.GetValue(Stat.ATK_RANGE) + 5);
                Vector3 point3DToReach = new(Point2DToReach.x, playerTransform.position.y, Point2DToReach.y);
                Vector3 direction = this.transform.position - point3DToReach;
                this.transform.forward = direction;
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
        yield return new WaitForSeconds(0.06f);
        isSmoothCoroutineOn = false;
    }

    private IEnumerator LongRangeAttack(Transform player)
    {
        // Stop bomber's attack
        canAttack = false;
        isAttacking = true;
        // Wait to face player
        GameObject bomb = Instantiate(pfBomb, this.transform.position, Quaternion.identity);
        ExplodingBomb exploBomb = bomb.GetComponent<ExplodingBomb>();
        float timeToThrow = 0.8f;
        Vector3 positionToReach = player.position;
        StartCoroutine(exploBomb.ThrowToPos(positionToReach, timeToThrow));
        exploBomb.SetTimeToExplode(timeToThrow * 1.5f);
        exploBomb.Activate();
        yield return new WaitForSeconds(1f);
        isAttacking = false;
        yield return new WaitForSeconds(timeBetweenAttack);
        canAttack = true;
    }



    public void ApplyDamage(int _value, bool isCrit = false, bool hasAnimation = true)
    {
        if (stats.GetValue(Stat.HP) <= 0)
            return;

        Stats.DecreaseValue(Stat.HP, _value, false);
        lifeBar.ValueChanged(stats.GetValue(Stat.HP));

        if (hasAnimation)
        {
            //add SFX here
            FloatingTextGenerator.CreateDamageText(_value, transform.position, isCrit);
            //AudioManager.Instance.PlaySound(hitSFX, transform.position);
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

        for (int i = 0; i < navPath.corners.Length; i++)
        {
            path.Add(navPath.corners[i]);
        }

        Debug.Log(navPath.corners.Length);

        float distance = Vector3.Distance(transform.position, posToReach);

        //for (int i = 1; i < nbDash; i++)
        //{
        //    // We avoid y value because we only move in x and z
        //    Vector2 posToReach2D = new(posToReach.x, posToReach.z);

        //    // Virtually get the "current" position of the dasher ( get the position he reached after his previous dash )
        //    Vector2 curPos2D = new(path[i - 1].x, path[i - 1].z);

        //    Vector2 direction = posToReach2D - curPos2D;
        //    Vector2 posOnCone = GetPointOnCone(curPos2D, direction, distance / nbDash, 60);
        //    Vector3 posOnCone3D = new Vector3(posOnCone.x, this.transform.position.y, posOnCone.y);
        //    NavMesh.SamplePosition(posOnCone3D, out var hit, float.PositiveInfinity, -1);
        //    posOnCone3D = hit.position;
        //    path.Add(posOnCone3D);
        //}
        // We finally add the position that we want to reach after every dash
        path.Add(posToReach);
        return path;
    }
    public Vector2 GetPointOnCircle(Vector2 center, float radius)
    {
        float randomValue = Random.Range(0, 2 * Mathf.PI);
        return new Vector2(center.x + Mathf.Cos(randomValue) * radius, center.y + Mathf.Sin(randomValue) * radius);
    }
    public Vector2 GetPointOnCone(Vector2 center, Vector2 direction, float radius, float angle)
    {
        float c = Mathf.Acos(direction.x / Mathf.Sqrt(direction.x * direction.x + direction.y * direction.y));
        float s = Mathf.Asin(direction.y / Mathf.Sqrt(direction.x * direction.x + direction.y * direction.y));
        float cs;
        float radAngle = angle * Mathf.Deg2Rad;
        if (s < 0)
        {
            if (c < Mathf.PI / 2)
                cs = s;
            else
                cs = Mathf.PI - s;
        }
        else
        {
            cs = c;
        }
        float randomValue = Random.Range(-radAngle + cs, radAngle + cs);
        return new Vector2(center.x + Mathf.Cos(randomValue) * radius, center.y + Mathf.Sin(randomValue) * radius);
    }
    public void Death()
    {
        OnDeath?.Invoke(transform.position);
        Destroy(gameObject);
        GameObject.FindWithTag("Player").GetComponent<Hero>().OnKill?.Invoke(this);
    }

    public void Attack(IDamageable damageable)
    {
        int damages = (int)stats.GetValue(Stat.ATK);
        onHit?.Invoke(damageable);
        damageable.ApplyDamage(damages);
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