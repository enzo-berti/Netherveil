using System.Collections;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public abstract class OverTimeStatus : Status
{
    private bool isCoroutineOn = false;
    protected float frequency = 1f;

    protected OverTimeStatus(float _duration, float _chance, float _frequency) : base(_duration, _chance)
    {
        frequency = _frequency;
    }
    public sealed override void DoEffect()
    {
        if (!isCoroutineOn)
        {
            CoroutineManager.Instance.StartCoroutine(EffectAsync());
        }
    }
    private IEnumerator EffectAsync()
    {
        isCoroutineOn = true;
        yield return new WaitForSeconds(frequency);
        if (!isFinished)
            Effect();
        isCoroutineOn = false;
    }

    public override void ApplyEffect(Entity target)
    {
        base.ApplyEffect(target);
        UpdateEffect();
    }

    private IEnumerator UpdateEffect()
    {
        while(true)
        {
            DoEffect();
            yield return null;
        }
    }
}
