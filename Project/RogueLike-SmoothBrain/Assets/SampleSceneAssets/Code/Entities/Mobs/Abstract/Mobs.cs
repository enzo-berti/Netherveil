using System.Collections;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(NavMeshAgent))]
public abstract class Mobs : Entity
{
    protected NavMeshAgent agent;
    protected SkinnedMeshRenderer skinnedMeshRenderer;
    protected Entity[] nearbyEntities;
    protected EnemyLifeBar lifeBar;

    protected override void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        lifeBar = GetComponentInChildren<EnemyLifeBar>();
        lifeBar.SetMaxValue(stats.GetValue(Stat.HP));

        nearbyEntities = null;
        ApplySpeed(Stat.SPEED);
        stats.onStatChange += ApplySpeed;
        OnDeath += cts => ClearStatus();

        if (this is IAttacker attacker)
        {
            attacker.OnHit += attacker.ApplyStatus;
        }

        StartCoroutine(EntityDetection());
        StartCoroutine(Brain());
    }

    private void ApplySpeed(Stat speedStat)
    {
        if (!speedStat.HasFlag(Stat.SPEED))
            return;

        agent.speed = stats.GetValue(Stat.SPEED);
    }

    protected virtual IEnumerator Brain()
    {
        yield return null;
    }

    protected virtual IEnumerator EntityDetection()
    {
        yield return null;
    }

    protected IEnumerator HitRoutine()
    {
        skinnedMeshRenderer.material.SetInt("_isHit", 1);
        yield return new WaitForSeconds(0.05f);
        skinnedMeshRenderer.material.SetInt("_isHit", 0);
        yield return new WaitForSeconds(0.05f);
        skinnedMeshRenderer.material.SetInt("_isHit", 1);
        yield return new WaitForSeconds(0.05f);
        skinnedMeshRenderer.material.SetInt("_isHit", 0);
    }

#if UNITY_EDITOR
    protected virtual void DisplayVisionRange(float _angle)
    {
        Handles.color = new Color(1, 0, 0, 0.25f);
        if (nearbyEntities != null && nearbyEntities.Length != 0)
        {
            Handles.color = new Color(0, 1, 0, 0.25f);
        }

        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, _angle / 2f, (int)stats.GetValue(Stat.VISION_RANGE));
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -_angle / 2f, (int)stats.GetValue(Stat.VISION_RANGE));

        Handles.color = Color.white;
        Handles.DrawWireDisc(transform.position, Vector3.up, (int)stats.GetValue(Stat.VISION_RANGE));
    }

    protected virtual void DisplayAttackRange(float _angle)
    {
        Handles.color = new Color(1, 1, 0.5f, 0.25f);
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, _angle / 2f, (int)stats.GetValue(Stat.ATK_RANGE));
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -_angle / 2f, (int)stats.GetValue(Stat.ATK_RANGE));

        Handles.color = Color.white;
        Handles.DrawWireDisc(transform.position, Vector3.up, (int)stats.GetValue(Stat.ATK_RANGE));
    }

    protected virtual void DisplayInfos()
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
                textColor = Color.black
            }
        });
    }
#endif
}