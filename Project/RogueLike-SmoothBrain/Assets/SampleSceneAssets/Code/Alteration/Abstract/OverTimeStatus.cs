using System.Collections;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public abstract class OverTimeStatus : Status
{
    private bool isCoroutineOn = false;
    protected float frequency;

    protected OverTimeStatus(float _duration, float _chance) : base(_duration, _chance)
    {
        isCoroutineOn = false;
    }
    public sealed override void DoEffect()
    {
        if (!isCoroutineOn)
        {
            CoroutineManager.Instance.StartCustom(EffectAsync());
        }
    }
    private IEnumerator EffectAsync()
    {
        isCoroutineOn = true;
        yield return new WaitForSeconds(frequency);
        if (!isFinished)
        {
            Effect();
        }
            
        isCoroutineOn = false;
    }

    public sealed override void ApplyEffect(Entity target)
    {
        base.ApplyEffect(target);
        CoroutineManager.Instance.StartCustom(UpdateEffect());
    }

    private IEnumerator UpdateEffect()
    {
        while(!isFinished)
        {
            DoEffect();
            yield return null;
        }
        yield break;
    }
}
