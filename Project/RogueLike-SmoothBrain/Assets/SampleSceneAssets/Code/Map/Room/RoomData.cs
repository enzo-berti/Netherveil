using System.Collections.Generic;
using UnityEngine;

namespace Map
{
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
        Boss
    }

    public struct RoomData
    {
        public RoomData(GameObject enemiesContainer)
        {
            enemies = new List<GameObject>();

            foreach (Mobs enemy in enemiesContainer.GetComponentsInChildren<Mobs>())
            {
                enemies.Add(enemy.gameObject);
            }

            Type = RoomType.Normal;
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
}