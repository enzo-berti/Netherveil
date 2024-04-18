using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.WSA;

public class DamnationVeil : ISpecialAbility
{
    public float Cooldown { get; set; } = 30f;
    public float CurrentEnergy { get; set; } = 0;
    private const float radius = 10f;

    public DamnationVeil()
    {
        CurrentEnergy = Cooldown;
    }
    public void Activate()
    {
        Physics.OverlapSphere(Utilities.Player.transform.position, radius, LayerMask.GetMask("Entity"))
            .Select(entity => entity.GetComponent<Mobs>())
            .Where(entity => entity != null)
            .ToList()
            .ForEach(currentEntity =>
            {
                Debug.Log("coucou");
                currentEntity.ApplyEffect(new Damnation(1000f, 1));
            });
    }
}
