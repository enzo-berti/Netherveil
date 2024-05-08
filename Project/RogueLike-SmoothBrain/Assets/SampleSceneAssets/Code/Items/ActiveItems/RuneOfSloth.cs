using Map;
using System.Linq;
using UnityEngine;

public class RuneOfSloth : ItemEffect, IActiveItem
{
    public float Cooldown { get; set; } = 30f;
    private readonly float duration = 3f;
    public void Activate()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.RuneOfSlothSFX,Utilities.Player.transform.position);
        Utilities.Player.GetComponent<PlayerController>().PlayVFX(Utilities.Player.GetComponent<PlayerController>().RuneOfSlothVFX);
        float planeLength = 5f;
        float radius = (Utilities.Player.GetComponent<PlayerController>().RuneOfSlothVFX.GetFloat("Diameter") * planeLength) / 2f;

        Physics.OverlapSphere(Utilities.Player.transform.position, radius, LayerMask.GetMask("Entity"))
            .Select(entity => entity.GetComponent<Mobs>())
            .Where(entity => entity != null)
            .ToList()
            .ForEach(currentEntity =>
            {
                currentEntity.AddStatus(new Freeze(duration, 1));
            });
    }
} 
