using System.Collections;
using UnityEngine;

public interface IActiveItem : IItem
{
    float Cooldown { get; set; }
    void Activate();
    sealed IEnumerator WaitToUse()
    {
        while((this as ItemEffect).CurrentEnergy < Cooldown)
        {
            (this as ItemEffect).CurrentEnergy += Time.deltaTime;
            yield return null;
        }
    }
}
