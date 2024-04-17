using Map;
using UnityEngine;

public class RuneOfSloth : ItemEffect, IActiveItem
{
    public float Cooldown { get; set; } = 20;
    private readonly float duration = 7;
    public void Activate()
    {
        foreach(var enemy in RoomUtilities.roomData.enemies)
        {
            if(enemy.TryGetComponent<Mobs>(out var mob))
            {
                mob.ApplyEffect(new Freeze(duration, 1));
            }
        }
    }
} 
