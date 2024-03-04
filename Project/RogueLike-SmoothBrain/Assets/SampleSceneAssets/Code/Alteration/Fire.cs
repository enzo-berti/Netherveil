using UnityEngine;

public class Fire : Status
{
    public int damage = 0;

    public Fire(int damage, float duration)
    {
        this.damage = damage;
        this.duration = duration;
    }

    public override void ApplyEffect(Entity target)
    {
        if (target.gameObject.TryGetComponent<IDamageable>(out _))
        {
            target.AddStatus(this);
        }
        else
        {
        }
    }

    public override void OnFinished()
    {
        target.Stats.SetCoeffValue(Stat.SPEED, 1f);
    }

    protected override void Effect()
    {
        target.gameObject.GetComponent<IDamageable>().ApplyDamage(damage);
        Debug.Log("damage");
    }
}
