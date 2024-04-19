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
            Debug.Log("Call effect in OvertimeStatus");
            Effect();
        }
            
        isCoroutineOn = false;
    }

    public override void ApplyEffect(Entity target)
    {
        base.ApplyEffect(target);
        Debug.Log("ApplyEffect in OvertimeStatus");
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
