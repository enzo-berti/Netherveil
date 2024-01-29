using System.Collections;
using UnityEngine;

public class Pest : Sbire, IAttacker, IDamageable, IMovable
{
    private IAttacker.AttackDelegate onAttack;
    private IAttacker.HitDelegate onHit;
    public IAttacker.AttackDelegate OnAttack { get => onAttack; set => onAttack = value; }
    public IAttacker.HitDelegate OnHit { get => onHit; set => onHit = value; }

    [Header("Pest Parameters")]
    [SerializeField] private float movementDelay = 2f;

    private IEnumerator MovementProcess()
    {
        while (true)
        {
            yield return new WaitForSeconds(movementDelay);

            //Entity[] entities = Physics.SphereCast()
        }
    }

    public void ApplyDamage(int _value)
    {
        throw new System.NotImplementedException();
    }

    public void Attack(IDamageable damageable)
    {
        throw new System.NotImplementedException();
    }

    public void Death()
    {
        throw new System.NotImplementedException();
    }

    public void MoveTo(Vector3 posToMove)
    {
        agent.SetDestination(posToMove);
    }
}
