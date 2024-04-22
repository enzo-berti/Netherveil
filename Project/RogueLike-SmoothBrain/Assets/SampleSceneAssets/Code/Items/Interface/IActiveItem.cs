using System;
using System.Collections;
using UnityEngine;

public interface IActiveItem : IItem
{
    public static event Action OnActiveItemCooldownStarted;
    public float Cooldown { get; set; }

    void Activate();
    sealed IEnumerator WaitToUse()
    {
        OnActiveItemCooldownStarted?.Invoke();
        while ((this as ItemEffect).CurrentEnergy < Cooldown)
        {
            (this as ItemEffect).CurrentEnergy += Time.deltaTime;
            yield return null;
        }
    }
}
