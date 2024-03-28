using Unity.AI.Navigation;
using UnityEngine;

public class RoomEvents : MonoBehaviour
{
    private RoomData mapData;

    private GameObject room;
    private GameObject enemies;
    private GameObject traps;
    private NavMeshSurface navMeshSurface;

    private void Start()
    {
        // find room go's
        room = transform.parent.Find("RoomGenerator").GetChild(0).gameObject;
        enemies = room.transform.Find("Enemies").gameObject;
        traps = room.transform.Find("Traps").gameObject;
        navMeshSurface = GetComponent<NavMeshSurface>();

        enemies.SetActive(false);

        // create data of the map
        mapData = new RoomData();
    }

    private void EnterEvents()
    {
        navMeshSurface.enabled = true;
        enemies.SetActive(true);
    }

    private void ExitEvents()
    {
        navMeshSurface.enabled = false;
        enemies.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            RoomUtilities.roomData = mapData;
            RoomUtilities.EnterEvents?.Invoke(ref mapData);
            EnterEvents();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            RoomUtilities.ExitEvents?.Invoke(ref mapData);
            ExitEvents();
        }
    }
}