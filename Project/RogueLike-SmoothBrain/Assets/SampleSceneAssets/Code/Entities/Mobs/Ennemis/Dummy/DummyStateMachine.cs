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

    private bool isAChargedAttack;
    private bool isAFinisherAttack;
    private bool isAnDistanceAttack;
    private bool isADashAttack;

    protected override void Start()
    {
        base.Start();
        hitHash = Animator.StringToHash("Hit");

        isAChargedAttack = false;
        isAFinisherAttack = false;
        isAnDistanceAttack = false;
        isADashAttack = false;

        Subscribe();
    }

    protected override void Update()
    {
        if (animator.speed == 0 || IsSpawning)
            return;

        base.Update();
    }

    private void SetDashAttack(IDamageable _damageable, IAttacker _attacker)
    {
        isADashAttack = true;
    }

    private void SetChargedAttack(IDamageable _damageable, IAttacker _attacker)
    {
        isAChargedAttack = true;
    }

    private void SetDistanceAttack(IDamageable _damageable, IAttacker _attacker)
    {
        isADashAttack = true;
    }

    private void SetFinisherAttack(IDamageable _damageable, IAttacker _attacker)
    {
        isAFinisherAttack = true;
    }

    private void CanTakeDamage(int _value)
    {
        if (isAFinisherAttack)
        {
            Stats.DecreaseValue(Stat.HP, _value, true);
        }
        else if (isAChargedAttack)
        {
            Stats.DecreaseValue(Stat.HP, _value, true);
        }
        else if (isAnDistanceAttack)
        {
            Stats.DecreaseValue(Stat.HP, _value, true);
        }
        else if (isADashAttack)
        {
            Stats.DecreaseValue(Stat.HP, _value, true);
        }
        lifeBar.ValueChanged(stats.GetValue(Stat.HP));
    }

    private void ResetBool()
    {
        isAFinisherAttack = false; 
        isAChargedAttack = false;
        isAnDistanceAttack = false;
        isADashAttack = false;
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
