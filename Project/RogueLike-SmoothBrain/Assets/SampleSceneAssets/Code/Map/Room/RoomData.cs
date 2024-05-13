using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    public enum RoomType
    {
        None,

        Lobby,
        Tutorial,
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
        public RoomData(Room roomPrefab, GameObject enemiesContainer)
        {
            this.enemiesContainer = enemiesContainer;
            Type = roomPrefab.type;
        }

        public readonly int NumEnemies
        {
            get
            {
                return Enemies.Count;
            }
        }

        public RoomType Type { get; private set; }

        private readonly GameObject enemiesContainer;
        public readonly List<GameObject> Enemies
        {
            get
            {
                List<GameObject> enemies = new List<GameObject>();

                foreach (Mobs enemy in enemiesContainer.GetComponentsInChildren<Mobs>())
                {
                    enemies.Add(enemy.gameObject);
                }

                return enemies;
            }
        }
    }
}