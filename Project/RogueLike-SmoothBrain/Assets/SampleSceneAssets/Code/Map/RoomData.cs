using System.Collections.Generic;
using UnityEngine;

public enum RoomType
{
    None,
    Lobby,
    Normal,
    Treasure,
    Challenge,
    Merchant,
    Secret,
    MiniBoss,
    Boss,

    COUNT
}

public struct RoomData
{
    public RoomData(GameObject enemiesContainer, Generation.RoomGenerator roomGenerator)
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