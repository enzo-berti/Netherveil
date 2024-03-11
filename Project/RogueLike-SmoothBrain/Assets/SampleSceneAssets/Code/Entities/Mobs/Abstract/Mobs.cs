using System.Collections;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;
using FMODUnity;

#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class Mobs : Entity
{
    protected NavMeshAgent agent;
    protected Rigidbody rb;
    protected Entity[] nearbyEntities;

    [SerializeField] EventReference deathSound;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        nearbyEntities = null;

        ApplySpeed(Stat.SPEED);
        stats.onStatChange += ApplySpeed;

        if (this is IAttacker attacker)
        {
            Debug.Log($"Apply status in {attacker}");
            attacker.OnHit += attacker.ApplyStatus;
        }
    }

    private void Start()
    {
        StartCoroutine(EntityDetection());
        StartCoroutine(Brain());
        OnDeath += ctx => PlayDeathSong();
        OnDeath += cts => ClearStatus();
    }

    private void ApplySpeed(Stat speedStat)
    {
        if (!speedStat.HasFlag(Stat.SPEED))
            return;

        agent.speed = stats.GetValue(Stat.SPEED);
    }

    protected abstract IEnumerator Brain();
    protected abstract IEnumerator EntityDetection();

    private void PlayDeathSong()
    {
        AudioManager.Instance.PlaySound(deathSound);
    }

    private void ClearStatus()
    {
        AppliedStatusList.Clear();
    }

#if UNITY_EDITOR
    virtual protected void DisplayVisionRange(float _angle)
    {
        Entity[] entities = PhysicsExtensions.OverlapVisionCone(transform.position, _angle, (int)stats.GetValue(Stat.VISION_RANGE), transform.forward)
           .Select(x => x.GetComponent<Entity>())
           .Where(x => x != null && x != this)
           .ToArray();

        Handles.color = new Color(1, 0, 0, 0.25f);
        if (entities.Length != 0)
        {
            Handles.color = new Color(0, 1, 0, 0.25f);
        }

        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, _angle / 2f, (int)stats.GetValue(Stat.VISION_RANGE));
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -_angle / 2f, (int)stats.GetValue(Stat.VISION_RANGE));

        Handles.color = Color.white;
        Handles.DrawWireDisc(transform.position, Vector3.up, (int)stats.GetValue(Stat.VISION_RANGE));
    }

    virtual protected void DisplayAttackRange(float _angle)
    {
        Handles.color = new Color(1, 1, 0.5f, 0.25f);
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, _angle / 2f, (int)stats.GetValue(Stat.ATK_RANGE));
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -_angle / 2f, (int)stats.GetValue(Stat.ATK_RANGE));

        Handles.color = Color.white;
        Handles.DrawWireDisc(transform.position, Vector3.up, (int)stats.GetValue(Stat.ATK_RANGE));
    }

    virtual protected void DisplayInfos()
    {
        Handles.Label(
    transform.position + transform.up,
    stats.GetEntityName() +
    "\n - Health : " + stats.GetValue(Stat.HP) +
    "\n - Speed : " + stats.GetValue(Stat.SPEED),
    new GUIStyle()
    {
        alignment = TextAnchor.MiddleLeft,
        normal = new GUIStyleState()
        {
            textColor = Color.white,
        }
    }); ;
    }
#endif
}