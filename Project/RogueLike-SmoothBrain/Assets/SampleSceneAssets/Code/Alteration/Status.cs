using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public abstract class Status
{
    public Status()
    {
        this.stopTimes.Add(this.duration);
    }
    public Status(Entity launcher)
    {
        this.duration *= launcher.Stats.GetValue(Stat.STATUS_DURATION);
        this.stopTimes.Add(this.duration);
    }

    public Entity target;
    protected bool isConst = false;
    
    // Frequency ( seconds )
    protected float frequency = 1f;

    public bool isFinished = false;

    protected float duration = 10;
    private float currentTime = 0;
    private List<float> stopTimes = new();

    private bool isCoroutineOn = false;

    protected int stack = 0;
    public int Stack { get => stack; }
    protected abstract void Effect();

    // Apply effect only if entity is effectable by this. Exemple, you won't apply Fire if entity doesn't have Idamageable
    public abstract void ApplyEffect(Entity target);

    // Do something when status is removed from the target
    public abstract void OnFinished();

    public virtual void AddStack(int nb)
    {
        stack += nb;
        stopTimes.Add(duration + currentTime);
    }

    public virtual void RemoveStack(int nb) 
    { 
        stack -= nb;
    }

    public void DoEffect()
    {
        if (!isFinished)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= stopTimes[0])
            {
                stopTimes.RemoveAt(0);
                stack--;
                if (stack <= 0)
                {
                    isFinished = true;
                    this.OnFinished();
                }
               
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
        await Task.Delay((int)(frequency * 1000));
        Effect();
        isCoroutineOn = false;
    }

}