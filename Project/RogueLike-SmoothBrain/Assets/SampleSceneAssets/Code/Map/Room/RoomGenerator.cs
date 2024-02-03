using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    public static int RoomGenerated { get; set; } = 0;

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
        int keepRoomIndex = (GameManager.Instance.seed.Range(0, rooms.Count) + RoomGenerator.RoomGenerated) % rooms.Count;
        for (int i = 0; i < rooms.Count; i++)
        {
            if (i != keepRoomIndex)
            {
                Destroy(rooms[i]);
            }
        }
    }
}