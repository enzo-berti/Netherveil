using UnityEngine;

public class DummyStateMachine : Mobs, IDummy
{
    [System.Serializable]
    private class DummySounds
    {
        public Sound hitSound;
    }

    enum Weakness
    {
        COMBO_FINISH,
        CHARGED_ATTACK,
        DISTANCE_ATTACK,
        DASH_ATTACK
    }

    private int hitHash;

    [SerializeField] private DummySounds dummySounds;

    [SerializeField] private Weakness weakness;
    [SerializeField] private GameObject objectToDestroy;

    private bool triggerAttack;

    protected override void Start()
    {
        base.Start();
        hitHash = Animator.StringToHash("Hit");

        triggerAttack = false;

        Subscribe();
    }

    protected override void Update()
    {
        if (animator.speed == 0 || IsSpawning)
            return;

        base.Update();
    }

    private void TriggerAttackBool(IDamageable _damageable, IAttacker _attacker)
    {
        triggerAttack = true;
    }

    private void CanTakeDamage(int _value)
    {
        if (triggerAttack)
        {
            Stats.DecreaseValue(Stat.HP, _value, true);
        }
        lifeBar.ValueChanged(stats.GetValue(Stat.HP));
    }

    public void ApplyDamage(int _value, IAttacker attacker, bool hasAnimation = true)
    {
        if (!triggerAttack)
            _value = 0;

        if (hasAnimation)
        {
            FloatingTextGenerator.CreateDamageText(_value, transform.position);
            StartCoroutine(HitRoutine());
        }

        animator.ResetTrigger(hitHash);
        animator.SetTrigger(hitHash);

        dummySounds.hitSound.Play(transform.position, true);

        CanTakeDamage(_value);
        triggerAttack = false;
        if (stats.GetValue(Stat.HP) <= 0)
        {
            Death();
        }
    }

    private void Subscribe()
    {
        switch (weakness)
        {
            case Weakness.COMBO_FINISH:
                Hero.OnFinisherAttack += TriggerAttackBool;
                break;
            case Weakness.CHARGED_ATTACK:
                Hero.OnChargedAttack += TriggerAttackBool;
                break;
            case Weakness.DISTANCE_ATTACK:
                Hero.OnSpearAttack += TriggerAttackBool;
                break;
            case Weakness.DASH_ATTACK:
                Hero.OnDashAttack += TriggerAttackBool;
                break;
        }
    }

    private void Unsubscribe()
    {
        switch (weakness)
        {
            case Weakness.COMBO_FINISH:
                Hero.OnFinisherAttack -= TriggerAttackBool;
                break;
            case Weakness.CHARGED_ATTACK:
                Hero.OnChargedAttack -= TriggerAttackBool;
                break;
            case Weakness.DISTANCE_ATTACK:
                Hero.OnSpearAttack -= TriggerAttackBool;
                break;
            case Weakness.DASH_ATTACK:
                Hero.OnDashAttack -= TriggerAttackBool;
                break;
        }
    }

    public void Death()
    {
        animator.speed = 1;
        Unsubscribe();
        Destroy(objectToDestroy);
        StopAllCoroutines();
    }
}
