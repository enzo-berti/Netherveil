using UnityEngine;

public class Fire : Status
{
    private int damage = 10;
    static Color fireColor = new Color(0.929f, 0.39f, 0.08f, 1);

    public Fire(float _duration) : base(_duration)
    {
        statusChance = 1.0f;
        frequency = 0.5f;
    }

    public override void ApplyEffect(Entity target)
    {
        if (target.gameObject.TryGetComponent<IDamageable>(out _))
        {
            this.stack++;
            target.AddStatus(this);
        }
    }

    public override void OnFinished()
    {
    }

    public override Status ShallowCopy()
    {
        return (Fire)this.MemberwiseClone();
    }

    protected override void Effect()
    {
        if (target != null)
        {
            FloatingTextGenerator.CreateEffectDamageText(damage * stack, target.transform.position, fireColor);
            target.gameObject.GetComponent<IDamageable>().ApplyDamage(damage * stack, false);
        }
    }
}
