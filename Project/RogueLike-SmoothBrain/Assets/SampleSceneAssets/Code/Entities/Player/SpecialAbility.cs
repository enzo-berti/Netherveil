using System.Collections;
using UnityEngine;

public interface ISpecialAbility
{
    public float Cooldown { get; set; }
    public float CurrentEnergy { get; set; }

    public void Activate();

    public IEnumerator WaitToUse()
    {
        while (CurrentEnergy < Cooldown)
        {
            CurrentEnergy += Time.deltaTime;
            yield return null;
        }
    }
}
