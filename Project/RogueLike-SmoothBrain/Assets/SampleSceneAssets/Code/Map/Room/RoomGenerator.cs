using System.Collections.Generic;
using UnityEngine;

namespace Generation
{
    public class RoomGenerator : MonoBehaviour
    {
        [HideInInspector] public RoomType type;

        private List<GameObject> Rooms
        {
            get
            {
                List<GameObject> rooms = new List<GameObject>();

                foreach (Transform child in transform)
                {
                    rooms.Add(child.gameObject);
                }

                return rooms;
            }
        }

        /// <summary>
        /// Choose between all child "room seeds".
        /// A room seed contain ennemies, treasures and props. Each one have a unique pattern.
        /// </summary>
        public void GenerateRoomSeed()
        {
            List<GameObject> rooms = Rooms;

#if UNITY_EDITOR
            if (rooms.Count <= 0)
            {
                Debug.LogError("Room doesn't have any seeds", gameObject);
                return;
            }
#endif

            int keepRoomIndex = Seed.Range(0, rooms.Count);
            for (int i = 0; i < rooms.Count; i++)
            {
                if (i != keepRoomIndex)
                {
                    DestroyImmediate(rooms[i]);
                }
            }

            rooms[keepRoomIndex].SetActive(true); // activate room
            Destroy(this); // doesn't need component anymore
        }
    }
}