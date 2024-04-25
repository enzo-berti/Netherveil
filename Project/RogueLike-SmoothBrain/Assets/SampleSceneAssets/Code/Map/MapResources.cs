using Map.Generation;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    public class MapResources : MonoBehaviour
    {
        static bool load = false;
        static private readonly Dictionary<RoomType, List<RoomPrefab>> roomPrefabsByType = new Dictionary<RoomType, List<RoomPrefab>>()
        {
            { RoomType.Lobby, new List<RoomPrefab>() },
            { RoomType.Normal, new List<RoomPrefab>() },
            { RoomType.Treasure, new List<RoomPrefab>() },
            { RoomType.Challenge, new List <RoomPrefab>() },
            { RoomType.Merchant, new List <RoomPrefab>() },
            { RoomType.Secret, new List <RoomPrefab>() },
            { RoomType.MiniBoss, new List <RoomPrefab>() },
            { RoomType.Boss, new List <RoomPrefab>() },
        };

        [SerializeField] private List<RoomPrefab> roomsToLoad;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void LoadGameResources()
        {
            GameObject.Instantiate(Resources.Load<GameObject>(nameof(MapResources)));
        }

        private void Awake()
        {
            if (!load)
            {
                foreach (RoomPrefab roomPrefab in roomsToLoad)
                {
                    roomPrefabsByType[roomPrefab.type].Add(roomPrefab);
                }
                roomsToLoad.Clear();
                roomsToLoad = null;

                load = true;
            }

            Destroy(gameObject);
        }

        public static RoomPrefab RandPrefabRoom(RoomType type)
        {
            List<RoomPrefab> roomPrefabs = roomPrefabsByType[type];

            if (roomPrefabs.Count == 0)
            {
                Debug.LogWarning("No room of type " + type + " in MapResources");
                return null;
            }

            return roomPrefabs[Seed.Range(0, roomPrefabs.Count)];
        }

        public static List<RoomPrefab> RoomPrefabs(RoomType listType)
        {
            return roomPrefabsByType[listType];
        }
    }
}