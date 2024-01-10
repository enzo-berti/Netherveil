using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    private void Awake()
    {
        List<GameObject> roomSeeds = new List<GameObject>();

        foreach (Transform child in transform)
        {
            roomSeeds.Add(child.gameObject);
        }
    }
}