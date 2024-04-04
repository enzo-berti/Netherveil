using System.Collections.Generic;
using UnityEngine;

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

    public void Generate()
    {
        GenerateRoomSeed();
    }

    /// <summary>
    /// Choose between all child "room seeds".
    /// RoomSeeds contain ennemies, treasures and props. Each one have a
    /// unique pattern.
    /// </summary>
    public void GenerateRoomSeed()
    {
        List<GameObject> rooms = Rooms;

        int keepRoomIndex = 0;
        if (rooms.Count > 1)
        {
            keepRoomIndex = Seed.Range(0, rooms.Count);
            for (int i = 0; i < rooms.Count; i++)
            {
                if (i != keepRoomIndex)
                {
                    DestroyImmediate(rooms[i]);
                }
            }
        }

        rooms[keepRoomIndex].SetActive(true); // activate room
    }
}