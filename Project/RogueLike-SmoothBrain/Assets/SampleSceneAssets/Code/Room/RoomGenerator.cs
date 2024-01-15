using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    private void Generate(int roomGenerated = 0)
    {
        GameManager.Instance.seed.Set(123456);

        List<GameObject> rooms = new List<GameObject>();

        foreach (Transform child in transform)
        {
            rooms.Add(child.gameObject);
        }

        int keepRoomIndex = (GameManager.Instance.seed.Range(0, rooms.Count) + roomGenerated) % rooms.Count;
        for (int i = 0; i < rooms.Count; i++)
        {
            if (i != keepRoomIndex)
            {
                Destroy(rooms[i]);
            }
        }
    }
}