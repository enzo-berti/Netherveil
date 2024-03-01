using System.Collections;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Tank : Mobs, IAttacker, IDamageable, IMovable, IBlastable
{
    private IAttacker.AttackDelegate onAttack;
    private IAttacker.HitDelegate onHit;
    public IAttacker.AttackDelegate OnAttack { get => onAttack; set => onAttack = value; }
    public IAttacker.HitDelegate OnHit { get => onHit; set => onHit = value; }

    [Header("Tank Parameters")]
    [SerializeField, Range(0f, 360f)] private float angle = 120f;
    [SerializeField] private float range = 5f;

    public void Attack(IDamageable damageable)
    {
        Destroy(gameObject);
    }

    public void ApplyDamage(int _value)
    {
        Stats.IncreaseValue(Stat.HP, -_value, false);
        DamageManager.Instance.CreateDamageText(_value, transform.position + Vector3.up * 2, 1);
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

    protected override IEnumerator Brain()
    {
        while (true)
        {
            yield return null;

            //    Hero player = PhysicsExtensions.OverlapVisionCone(transform.position, angle, range, transform.forward)
            //        .Select(x => x.GetComponent<Hero>())
            //        .Where(x => x != null)
            //        .FirstOrDefault();

            //    if (player)
            //    {
            //        // Player detect
            //        if (agent.velocity.magnitude == 0f && Vector3.Distance(transform.position, player.transform.position) < 2f)
            //        {
            //            // Do attack
            //        }
            //        else
            //        {
            //            MoveTo(player.transform.position);
            //        }
            //    }
        }
    }

    protected override IEnumerator EntityDetection()
    {
        while (true)
        {
            yield return null;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (Selection.activeGameObject != gameObject)
            return;

        DisplayVisionRange(angle);
        DisplayAttackRange(angle);
        DisplayInfos();
    }
#endif
}

// |--------------|
// |TANK BEHAVIOUR|
// |--------------|
// If Player detect
//  Move to Player
// Else
//  Don't move