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
        static public Dictionary<RoomType, int> nbRoomByType = new Dictionary<RoomType, int>
        {
            { RoomType.Lobby, 0 },
            { RoomType.Normal, 0 },
            { RoomType.Treasure, 0 },
            { RoomType.Challenge, 0 },
            { RoomType.Merchant, 0 },
            { RoomType.Secret, 0 },
            { RoomType.MiniBoss, 0 },
            { RoomType.Boss, 0 },
        };

        public readonly int NbRoom
        {
            get
            {
                int totalCount = 0;
                foreach (int count in nbRoomByType.Values)
                {
                    totalCount += count;
                }

                return totalCount;
            }
        }

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
}