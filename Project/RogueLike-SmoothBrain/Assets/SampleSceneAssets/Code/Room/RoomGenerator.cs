using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    private readonly List<GameObject> roomSeeds = new List<GameObject>();

    private void Awake()
    {
        foreach (Transform child in transform)
        {
            roomSeeds.Add(child.gameObject);
        }
    }
}