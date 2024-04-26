using System;
using System.Collections;
using UnityEngine;

public interface IActiveItem : IItem
{
    public static event Action<ItemEffect> OnActiveItemCooldownStarted;
    public float Cooldown { get; set; }

    void Activate();
    sealed IEnumerator WaitToUse()
    {
        ItemEffect effect = this as ItemEffect;

        OnActiveItemCooldownStarted?.Invoke(effect);
        while (effect.CurrentEnergy < Cooldown)
        {
            effect.CurrentEnergy += Time.deltaTime;
            yield return null;
        }
    }
}
