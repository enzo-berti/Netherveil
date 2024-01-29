using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum DoorState : byte
{
    NONE = 0,
    OPEN,
    CLOSE
}

public class DoorsGenerator : MonoBehaviour
{

    public static int RandGenerator { get; private set; } = 0;
    public List<GameObject> doors { get; private set; } = new List<GameObject>();
    [SerializeField, MinMaxSlider(1, 4)] private Vector2Int minMaxDoors;

    public int NbAvailableDoors
    {
        get
        {
            return doors.Count;
        }
    }

    // TOTALEMENT BROKEN
    private void OnValidate()
    {
        foreach (Transform child in transform)
        {
            doors.Add(child.gameObject);
        }

        if (minMaxDoors.y > doors.Count)
        {
            minMaxDoors.y = doors.Count;
        }
        if (minMaxDoors.x < 0)
        {
            minMaxDoors.x = 0;
        }
    }

    private void Awake()
    {
        foreach (Transform child in transform)
        {
            doors.Add(child.gameObject);
        }
    }

    public void GenerateDoors(GenerationParameters generationParameters)
    {
        // TODO : ADD RANDOM VALUE BETWEEN (x, y) 

        // get number of doors that can spawn depending on the number of rooms available by genParams
        int numDoor = NbAvailableDoors - Mathf.Clamp(NbAvailableDoors - generationParameters.NbRoom, minMaxDoors.x, minMaxDoors.y);

    }

    public GameObject GetRdmDoor()
    {
        int index = (GameManager.Instance.seed.Range(0, doors.Count) + DoorsGenerator.RandGenerator) % doors.Count;

        return doors[index];
    }

    public void CloseDoor(int index)
    {
        GameObject go = doors[index];
        doors.RemoveAt(index);

        // TODO : Close the room (h word)
        Destroy(go);
    }

    public void CloseDoor(GameObject door)
    {
        if (doors.Remove(door))
        {
            // TODO : close the room (h word)
            Destroy(door);
        }

        Debug.LogWarning("Try to set a door state with the wrong GameObject in " + gameObject, door);
    }
}
