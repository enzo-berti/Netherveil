using System.Collections.Generic;
using UnityEngine;

public struct RoomData
{
    public RoomData(GameObject enemiesContainer, RoomGenerator roomGenerator)
    {
        enemies = new List<GameObject>();

        foreach (Mobs enemy in enemiesContainer.GetComponentsInChildren<Mobs>())
        {
            enemies.Add(enemy.gameObject);
        }

        Type = roomGenerator.type;
    }

    public readonly int NumEnemies
    {
        get
        {
            return enemies.Count;
        }
    }

    public RoomType Type { get; private set; }

    public List<GameObject> enemies;
}