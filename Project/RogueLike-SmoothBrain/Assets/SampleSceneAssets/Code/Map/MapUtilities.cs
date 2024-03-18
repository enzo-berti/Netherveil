using UnityEngine;

public class MapUtilities : MonoBehaviour
{
    public void AddRoomsEnterEvent(RoomEvents.Enter enter)
    {
        foreach (RoomEvents room in transform.GetComponentsInChildren<RoomEvents>())
        {
            room.AddEnterEvent(enter);
        }
    }

    public void AddRoomsExitEvent(RoomEvents.Exit exit)
    {
        foreach (RoomEvents room in transform.GetComponentsInChildren<RoomEvents>())
        {
            room.AddExitEvent(exit);
        }
    }
}