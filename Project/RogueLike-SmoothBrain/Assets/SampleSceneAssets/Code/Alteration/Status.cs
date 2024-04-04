using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

[Serializable]
public abstract class Status
{
    public Status(float _duration)
    {
        this.duration = _duration;
        AddStack(1);
    }
    public abstract Status DeepCopy();

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

    protected bool isStackable = false;
    #endregion

    #region Time
    public bool isFinished = false;
    private float currentTime = 0;

    readonly public List<float> stopTimes = new();
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
    private int stack = 0;
    public int Stack { get => stack; }
    public virtual void AddStack(int nb)
    {
        if(isStackable || stack < 1)
        {
            stack += isStackable ? nb : 1;
            for (int i = 0; i < nb; i++)
                stopTimes.Add(duration + currentTime);
        }
        
    }
    public virtual void RemoveStack(int nb)
    {
        if(isStackable)
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

    protected void PlayVfx(string vfxName)
    {
        if (target.transform.parent.Find(vfxName) == null)
        {
            VisualEffect vfx = GameObject.Instantiate(Resources.Load<GameObject>(vfxName), target.transform.parent).GetComponent<VisualEffect>();
            vfx.gameObject.transform.position = Vector3.zero;
            vfx.gameObject.GetComponent<VFXStopper>().Duration = duration;
            vfx.SetSkinnedMeshRenderer("New SkinnedMeshRenderer", target.gameObject.GetComponentInChildren<SkinnedMeshRenderer>());
            vfx.GetComponent<VFXPropertyBinder>().GetPropertyBinders<VFXTransformBinderCustom>().ToArray()[0].Target = target.gameObject.GetComponentInChildren<VFXTarget>().transform;
            vfx.gameObject.GetComponent<VFXStopper>().PlayVFX();
        }
        else
        {
            VFXStopper vfxStopper = target.transform.parent.Find(vfxName).GetComponent<VFXStopper>();
            vfxStopper.StopAllCoroutines();
            vfxStopper.PlayVFX();
            vfxStopper.Duration = duration;
        }
    }

    private async void EffectAsync()
    {
        isCoroutineOn = true;
        await Task.Delay((int)(frequency * 1000));
        if (!isFinished)
            Effect();
        isCoroutineOn = false;
    }
}