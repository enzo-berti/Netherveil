using UnityEngine;

public class DummyStateMachine : Mobs, IDummy
{
    [System.Serializable]
    private class DummySounds
    {
        public Sound hitSound;
    }

    private int hitHash;

    [SerializeField] private DummySounds dummySounds;

    [HideInInspector]
    protected override void Start()
    {
        base.Start();
        hitHash = Animator.StringToHash("Hit");
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
    }

    public void Death()
    {
        // Can't die
    }
}
