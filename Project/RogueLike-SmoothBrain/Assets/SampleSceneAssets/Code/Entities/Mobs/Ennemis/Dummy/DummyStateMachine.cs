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

    private bool isAnChargedAttack;
    private bool isAnFinisherAttack;
    private bool isAnDistanceAttack;
    private bool isAnDashAttack;

    protected override void Start()
    {
        base.Start();
        hitHash = Animator.StringToHash("Hit");

        isAnChargedAttack = false;
        isAnFinisherAttack = false;
        isAnDistanceAttack = false;
        isAnDashAttack = false;

        Subscribe();
    }

    protected override void Update()
    {
        if (animator.speed == 0)
            return;

        base.Update();
    }

    private void SetDashAttack(IDamageable _damageable, IAttacker _attacker)
    {
        isAnDashAttack = true;
    }

    private void SetChargedAttack(IDamageable _damageable, IAttacker _attacker)
    {
        isAnChargedAttack = true;
    }

    private void SetDistanceAttack(IDamageable _damageable, IAttacker _attacker)
    {
        isAnDashAttack = true;
    }

    private void SetFinisherAttack(IDamageable _damageable, IAttacker _attacker)
    {
        isAnFinisherAttack = true;
    }

    private void CanTakeDamage(int _value)
    {
        if (isAnFinisherAttack)
        {
            Stats.DecreaseValue(Stat.HP, _value, true);
        }
        if (isAnChargedAttack)
        {
            Stats.DecreaseValue(Stat.HP, _value, true);
        }
        if (isAnDistanceAttack)
        {
            Stats.DecreaseValue(Stat.HP, _value, true);
        }
        if (isAnDashAttack)
        {
            Stats.DecreaseValue(Stat.HP, _value, true);
        }
        lifeBar.ValueChanged(stats.GetValue(Stat.HP));
    }

    private void ResetBool()
    {
        isAnFinisherAttack = false; 
        isAnChargedAttack = false;
        isAnDistanceAttack = false;
        isAnDashAttack = false;
    }

    public void ApplyDamage(int _value, IAttacker attacker, bool hasAnimation = true)
    {
        if (hasAnimation)
        {
            FloatingTextGenerator.CreateDamageText(_value, transform.position);
            StartCoroutine(HitRoutine());
        }

        animator.ResetTrigger(hitHash);
        animator.SetTrigger(hitHash);

        dummySounds.hitSound.Play(transform.position, true);

        CanTakeDamage(_value);
        ResetBool();
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
                Hero.OnFinisherAttack += SetFinisherAttack;
                break;
            case Weakness.CHARGED_ATTACK:
                Hero.OnChargedAttack += SetChargedAttack;
                break;
            case Weakness.DISTANCE_ATTACK:
                Hero.OnSpearAttack += SetDistanceAttack;
                break;
            case Weakness.DASH_ATTACK:
                Hero.OnDashAttack += SetDashAttack;
                break;
        }
    }

    private void Unsubscribe()
    {
        switch (weakness)
        {
            case Weakness.COMBO_FINISH:
                Hero.OnFinisherAttack -= SetFinisherAttack;
                break;
            case Weakness.CHARGED_ATTACK:
                Hero.OnChargedAttack -= SetChargedAttack;
                break;
            case Weakness.DISTANCE_ATTACK:
                Hero.OnSpearAttack -= SetDistanceAttack;
                break;
            case Weakness.DASH_ATTACK:
                Hero.OnDashAttack -= SetDashAttack;
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
