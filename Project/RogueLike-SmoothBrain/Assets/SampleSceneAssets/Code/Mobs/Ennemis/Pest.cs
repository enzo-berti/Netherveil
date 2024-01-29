using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Pest : Sbire, IAttacker, IDamageable, IMovable
{
    private IAttacker.AttackDelegate onAttack;
    private IAttacker.HitDelegate onHit;
    public IAttacker.AttackDelegate OnAttack { get => onAttack; set => onAttack = value; }
    public IAttacker.HitDelegate OnHit { get => onHit; set => onHit = value; }

    [Header("Pest Parameters")]
    [SerializeField] private float movementDelay = 2f;
    [SerializeField, Range(0f, 360f)] private float angle = 120f;
    [SerializeField] private float range = 5f;
    [SerializeField] private float damages = 5f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = stats.GetValueStat(Stat.SPEED);

        StartCoroutine(MovementProcess());
    }

    private IEnumerator MovementProcess()
    {
        while (true)
        {
            yield return new WaitForSeconds(movementDelay);

            Entity[] entities = PhysicsExtensions.OverlapVisionCone(transform.position, angle, range, transform.forward)
                .Select(x => x.GetComponent<Entity>())
                .Where(x => x != null && x != this)
                .OrderBy(x => Vector3.Distance(x.transform.position, transform.position))
                .ToArray();

            Hero player = entities
                .Select(x => x.GetComponent<Hero>())
                .Where(x => x != null)
                .FirstOrDefault();

            Pest[] pests = entities
                .Select(x => x.GetComponent<Pest>())
                .Where(x => x != null)
                .ToArray();

            if (player)
            {
                // Player detect
                MoveTo(player.transform.position);
            }
            else if (pests.Any())
            {
                // Other pest detect
                MoveTo(pests.First().transform.position);
            }
            else
            {
                // Random movement
                Vector2 rdmPos = Random.insideUnitCircle * range;
                MoveTo(transform.position + new Vector3(rdmPos.x, 0, rdmPos.y));
            }
        }
    }

    public void Attack(IDamageable damageable)
    {
        damageable.ApplyDamage((int)damages);
    }

    public void ApplyDamage(int _value)
    {
        Stats.IncreaseValue(Stat.HP, -_value);
        if (stats.GetValueStat(Stat.HP) <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        Destroy(gameObject);
    }

    public void MoveTo(Vector3 posToMove)
    {
        agent.SetDestination(posToMove);
    }

    private void OnCollisionEnter(Collision collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

        if (!collision.gameObject.CompareTag("Player") || damageable == null)
            return;

        Attack(damageable);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        //if (Selection.activeGameObject != gameObject)
        //    return;

        Entity[] entities = PhysicsExtensions.OverlapVisionCone(transform.position, angle, range, transform.forward)
            .Select(x => x.GetComponent<Entity>())
            .Where(x => x != null && x != this)
            .ToArray();

        Handles.color = new Color(1, 0, 0, 0.25f);
        if (entities.Length != 0)
        {
            Handles.color = new Color(0, 1, 0, 0.25f);
        }

        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, angle / 2f, range);
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -angle / 2f, range);

        Handles.color = Color.white;
        Handles.DrawWireDisc(transform.position, Vector3.up, range);
    }
#endif
}

// |--------------|
// |PEST BEHAVIOUR|
// |--------------|
// If Player detect
//  Attack Player
// Else if Pest detect
//  Follow Pest
// Else
//  Random jump