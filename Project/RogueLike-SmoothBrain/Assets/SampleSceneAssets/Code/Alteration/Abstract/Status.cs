using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

[Serializable]
public abstract class Status
{
    public IAttacker launcher = null;
    public Entity target;
    public Status(float _duration, float _chance)
    {
        this.duration = _duration;
        this.statusChance = _chance;
        this.totalDuration = duration;
        AddStack(1);
        CoroutineManager.Instance.StartCoroutine(ManageStack());
    }

    public abstract Status DeepCopy();

    #region Properties
    
    // If an effect is played cyclically, at which frequency ( in seconds )
    
    // Chance to apply a status ( 0 -> 1 )
    public float statusChance = 0.3f;
    // Duration of one stack of the effect
    protected float duration = 1;
    private float totalDuration = 0;
    protected bool isStackable = false;
    #endregion

    #region Time
    public bool isFinished = false;
    protected float currentTime = 0;

    readonly public List<float> stopTimes = new();

    #endregion

    protected event Action OnAddStack;
    #region Abstract Effect Functions
    protected abstract void Effect();
    public abstract bool CanApplyEffect(Entity target);
    public virtual void ApplyEffect(Entity target)
    {
        PlayVFX();
    }

    // Do something when status is removed from the target
    public abstract void OnFinished();
    protected abstract void PlayVFX();
    #endregion

    #region Stack
    protected int stack = 0;
    protected int maxStack = int.MaxValue;
    //public event Action onAddingStack;
    public int Stack { get => stack; }
    public void AddStack(int nb)
    {
        if((isStackable && stack < maxStack) || stack < 1)
        {
            stack += isStackable ? nb : 1;
            for (int i = 0; i < nb; i++)
            {
                stopTimes.Add(duration + currentTime);
                totalDuration = stopTimes.Sum();
                OnAddStack?.Invoke();
            }
        }
        
    }
    public void RemoveStack(int nb)
    {
        if(isStackable)
            stack -= nb;
    }
    private IEnumerator ManageStack()
    {
        while(true)
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
                        OnFinished();
                    }
                }
            }
            yield return null;
        }
    }
    #endregion

    // Update and play effect on a target
    public abstract void DoEffect();

    protected void PlayVfx(string vfxName)
    {
        if (target.transform.parent.Find(vfxName) == null)
        {
            VisualEffect vfx = GameObject.Instantiate(GameResources.Get<GameObject>(vfxName), target.transform.parent).GetComponent<VisualEffect>();
            vfx.gameObject.transform.position = Vector3.zero;
            vfx.gameObject.GetComponent<VFXStopper>().Duration = totalDuration;
            vfx.SetSkinnedMeshRenderer("New SkinnedMeshRenderer", target.gameObject.GetComponentInChildren<SkinnedMeshRenderer>());
            vfx.GetComponent<VFXPropertyBinder>().GetPropertyBinders<VFXTransformBinderCustom>().ToArray()[0].Target = target.gameObject.GetComponentInChildren<VFXTarget>().transform;
            vfx.gameObject.GetComponent<VFXStopper>().PlayVFX();
        }
        else
        {
            VFXStopper vfxStopper = target.transform.parent.Find(vfxName).GetComponent<VFXStopper>();
            vfxStopper.StopAllCoroutines();
            vfxStopper.Duration = totalDuration;
            vfxStopper.PlayVFX();
        }
    }

    
}