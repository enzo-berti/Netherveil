using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
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

    public void Generate(int numberOfDoors = 1)
    {
        // DEBUG GameManager.Instance.seed.Set(123456);

        GenerateRoomSeed();
    }

    private void GenerateRoomSeed()
    {
        List<GameObject> rooms = Rooms;
        int keepRoomIndex = (GameManager.Instance.seed.Range(0, rooms.Count) + MapGenerator.RoomGenerated) % rooms.Count;
        for (int i = 0; i < rooms.Count; i++)
        {
            if (i != keepRoomIndex)
            {
                Destroy(rooms[i]);
            }
        }
    }
}