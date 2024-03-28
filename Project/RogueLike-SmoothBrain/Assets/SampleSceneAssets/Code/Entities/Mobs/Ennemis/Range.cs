using System.Collections;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
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

    [SerializeField] GameObject pfBomb;
    [Header("Range Parameters")]
    [SerializeField, Min(0)] private float staggerDuration;


    private float staggerImmunity;
    private float staggerTimer;

    private bool canAttack = true;
    private bool isFleeing = false;
    private bool isGoingOnPlayer = false;
    private bool isAttacking = false;
    private bool atPosToAttack = false;
    private float DistanceToFlee
    {
        get => stats.GetValue(Stat.ATK_RANGE) / 2f;
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

    private bool IsTurnToPlayer()
    {
        return false;
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
                // Il se tourne vers le player pour l'attaquer
                //StartCoroutine(FaceToPlayer(player.transform));
                this.transform.LookAt(playerTransform.transform.position);
                // Quand il est tourné vers le player il lui envoie une bombe
                StartCoroutine(LongRangeAttack(playerTransform.transform));
            }
            // Si l'ennemi peut attaquer il va se deplacer à range du player
            else if (!isGoingOnPlayer && !isFleeing && canAttack && !isAttacking)
            {
                Debug.Log("Going on player");
                isGoingOnPlayer = true;
                atPosToAttack = false;
                Vector2 playerPosXZ = new Vector2(playerTransform.position.x, playerTransform.position.z);
                // Minus an offset to avoid the enemy to have to go on the player each time this one move 
                Vector2 Point2DToReach = GetPointOnCircle(playerPosXZ, stats.GetValue(Stat.ATK_RANGE) - 3f);
                Vector3 point3DToReach = new Vector3(Point2DToReach.x, playerTransform.position.y, Point2DToReach.y);
                MoveTo(point3DToReach);
            }
            // Flee
            else if (!isGoingOnPlayer && !isFleeing && !isAttacking && distanceFromPlayer <= DistanceToFlee)
            {
                Debug.Log("fleeing");
                Vector2 playerPosXZ = new Vector2(playerTransform.position.x, playerTransform.position.z);
                Vector2 Point2DToReach = GetPointOnCircle(playerPosXZ, stats.GetValue(Stat.ATK_RANGE) + 10);
                Vector3 point3DToReach = new Vector3(Point2DToReach.x, playerTransform.position.y, Point2DToReach.y);
                Vector3 direction = this.transform.position - point3DToReach;
                this.transform.forward = direction;
                MoveTo(point3DToReach);
                isFleeing = true;
            }
            
            if (!agent.hasPath)
            {
                if(isFleeing)
                {
                    isFleeing = false; 
                    this.transform.LookAt(playerTransform.transform.position);
                }
                if(isGoingOnPlayer)
                {
                    isGoingOnPlayer = false;
                }
            }

            //UpdateStates();
        }
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
        yield return new WaitForSeconds(1f);
        canAttack = true;
    }

    public Vector2 GetPointOnCircle(Vector2 center, float radius)
    {
        float randomValue = Random.Range(0, 2 * Mathf.PI);
        return new Vector2(center.x + Mathf.Cos(randomValue) * radius, center.y + Mathf.Sin(randomValue) * radius);
    }
    public IEnumerator FaceToPlayer(Transform player)
    {
        while (!IsTurnToPlayer())
        {
            yield return null;
            this.transform.LookAt(player.transform.position);
            //float timer = 0f;
            //Vector3 playerPos = player.position;
            //Vector3 targetToThis = (playerPos - this.transform.position).normalized;
            //Quaternion rotation;
            //Vector3 baseRotation = this.transform.eulerAngles;
            //Vector3 finalRotation = baseRotation;
            //if (targetToThis != Vector3.zero)
            //{
            //    rotation = Quaternion.LookRotation(new Vector3(targetToThis.x, 0, targetToThis.z));
            //    rotation *= Camera.main.transform.rotation;
            //    float rotY = rotation.eulerAngles.y;

            //    finalRotation.y = rotY;
            //}
            //while (timer < 1f)
            //{
            //    this.transform.eulerAngles = Vector3.Lerp(baseRotation, finalRotation, timer);
            //    timer += Time.deltaTime * 3;
            //    yield return null;
            //}
        }

    }

    public void ApplyDamage(int _value, bool isCrit = false, bool hasAnimation = true)
    {
        Stats.DecreaseValue(Stat.HP, _value, false);

        lifeBar.ValueChanged(stats.GetValue(Stat.HP));
        FloatingTextGenerator.CreateDamageText(_value, transform.position, isCrit);
        if (hasAnimation)
        {
            //add SFX here
        }

        if (stats.GetValue(Stat.HP) <= 0)
        {
            Death();
        }
    }

    //void UpdateStates()
    //{
    //    if (isFighting)
    //    {
    //        state = RangeState.TRIGGERED;
    //    }
    //    else
    //    {
    //        state = RangeState.WANDERING;
    //    }
    //}
    public List<Vector3> GetDashesPath(Vector3 posToReach, int nbDash)
    {
        List<Vector3> path = new List<Vector3>();
        return new List<Vector3>();
    }
    public void Death()
    {
        OnDeath?.Invoke(transform.position);
        Destroy(gameObject);
        GameObject.FindWithTag("Player").GetComponent<Hero>().OnKill?.Invoke(this);
    }

    public void Attack(IDamageable damageable)
    {
        int damages = (int)(stats.GetValue(Stat.ATK) * stats.GetValue(Stat.ATK_COEFF));
        onHit?.Invoke(damageable);
        damageable.ApplyDamage(damages);
    }

    public void MoveTo(Vector3 posToMove)
    {
        agent.SetDestination(posToMove);
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