using UnityEngine;

public class Fire : Status
{
    private int damage = 10;
    static Color fireColor = new Color(0.929f, 0.39f, 0.08f, 1.0f);

    public Fire(float _duration) : base(_duration)
    {
        statusChance = 1.0f;
        frequency = 0.5f;
    }
    public Fire(float _duration, float _statusChance) : base(_duration)
    {
        statusChance = _statusChance;
        frequency = 0.5f;
    }

    public override void ApplyEffect(Entity target)
    {
        if (target.gameObject.TryGetComponent<IDamageable>(out _))
        {
            AddStack(1);
            target.AddStatus(this);

            PlayVfx("VFX_Fire");
        }
    }

    public override void OnFinished()
    {
        StopVfx();
    }

    protected override void Effect()
    {
        if (target != null)
        {
            FloatingTextGenerator.CreateEffectDamageText(damage * Stack, target.transform.position, fireColor);
            target.gameObject.GetComponent<IDamageable>().ApplyDamage(damage * Stack, false, false);
        }
    }

    public override Status DeepCopy()
    {
        Fire fire = (Fire)this.MemberwiseClone();
        return fire;
    }
}
