using System.Collections.Generic;
using UnityEngine;

public struct RoomData
{
    public RoomData(GameObject enemiesContainer, GameObject room)
    {
        enemies = new List<GameObject>();

        foreach (Transform enemyTransform in enemiesContainer.transform)
        {
            enemies.Add(enemyTransform.gameObject);
        }

        Type = room.GetComponent<RoomGenerator>().type;
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