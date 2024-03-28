using System.Collections.Generic;
using UnityEngine;

public struct RoomData
{
    public RoomData(GameObject enemiesContainer)
    {
        enemies = new List<GameObject>();

        foreach (Transform enemyTransform in enemiesContainer.transform)
        {
            enemies.Add(enemyTransform.gameObject);
        }
    }

    public List<GameObject> enemies;
}