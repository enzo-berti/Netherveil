using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    private static int RoomGenerated = 0; // rand noise

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
        // DEBUG GameManager.Instance.seed.Set(123456);

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
            keepRoomIndex = GameAssets.Instance.seed.Range(0, rooms.Count, ref RoomGenerated);

            for (int i = 0; i < rooms.Count; i++)
            {
                if (i != keepRoomIndex)
                {
                    Destroy(rooms[i]);
                }
            }
        }

        rooms[keepRoomIndex].SetActive(true); // activate room
    }
}