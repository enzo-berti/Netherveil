using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public abstract class Status
{
    public Status(float _duration)
    {
        this.duration = _duration;
        this.stopTimes.Add(duration);
    }
    public abstract Status ShallowCopy();

    #region Properties
    public Entity target;
    // If an effect is not played cyclically
    protected bool isConst = false;
    // If an effect is played cyclically, at which frequency ( in seconds )
    protected float frequency = 1f;
    // Chance to apply a status ( 0 -> 1 )
    public float statusChance = 0.3f;
    // Duration of one stack of the effect
    protected float duration = 1;
    #endregion

    #region Time
    public bool isFinished = false;
    private float currentTime = 0;

    readonly private List<float> stopTimes = new();
    private bool isCoroutineOn = false;
    #endregion

    #region Abstract Effect Functions
    protected abstract void Effect();

    // Apply effect only if entity is effectable by this. Exemple, you won't apply Fire if entity doesn't have Idamageable
    public abstract void ApplyEffect(Entity target);

    // Do something when status is removed from the target
    public abstract void OnFinished();
    #endregion
    
    #region Stack
    protected int stack = 0;
    public int Stack { get => stack; }
    public virtual void AddStack(int nb)
    {
        stack += nb;
        stopTimes.Add(duration + currentTime);
    }
    public virtual void RemoveStack(int nb)
    {
        stack -= nb;
    }
    #endregion

    // Update and play effect on a target
    public void DoEffect()
    {
        if (!isFinished && stopTimes.Count > 0)
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