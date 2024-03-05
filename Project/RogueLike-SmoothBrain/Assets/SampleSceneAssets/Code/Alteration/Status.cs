using System;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public abstract class Status
{
    public Entity target;
    protected bool isConst = false;
    protected float duration = 10;
    // Frequency ( seconds )
    protected float frequency = 1f;

    public bool isFinished = false;

    private float currentTime = 0;
    private bool isCoroutineOn = false;
    protected abstract void Effect();

    // Apply effect only if entity is effectable by this. Exemple, you won't apply Fire if entity doesn't have Idamageable
    public abstract void ApplyEffect(Entity target);
    public abstract void OnFinished();
    public void DoEffect()
    {
        
        if (!isFinished)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= duration)
            {
                isFinished = true;
                this.OnFinished();
            }
            if (isConst) return;
            if (!isCoroutineOn)
            {
               EffectAsync();
            }
            
        }

    }
    private async void EffectAsync()
    {
        isCoroutineOn = true;
        await Task.Delay((int)frequency * 1000);
        Effect();
        isCoroutineOn = false;
    }

    
}