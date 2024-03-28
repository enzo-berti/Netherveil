using UnityEngine;

public class Freeze : Status
{
    public Freeze(float duration) : base(duration)
    {
        isConst = true;
    }

    private int damage = 10;
    static Color freezeColor = new Color(0.11f, 0.78f, 0.87f, 1.0f);

    public override void ApplyEffect(Entity target)
    {
        if (target.Stats.HasStat(Stat.SPEED))
            target.AddStatus(this);
    }

    public override Status DeepCopy()
    {
        Freeze fire = (Freeze)MemberwiseClone();
        return fire;
    }

    public override void OnFinished()
    {

    }


    protected override void Effect()
    {
        if (target != null)
        {
            FloatingTextGenerator.CreateEffectDamageText(damage * Stack, target.transform.position, freezeColor);
            target.gameObject.GetComponent<IDamageable>().ApplyDamage(damage * Stack, false, false);
        }
    }
}
