using UnityEngine;

public class RoomEvents : MonoBehaviour
{
    public delegate void Enter(ref MapData mapData);
    public delegate void Exit(ref MapData mapData);

    MapData mapData = new MapData();

    private void EnterEvents()
    {
        transform.parent.Find("RoomGenerator").gameObject.SetActive(true);
    }

    private void ExitEvents()
    {
        transform.parent.Find("RoomGenerator").gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            MapUtilities.EnterEvents?.Invoke(ref mapData);
            EnterEvents();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            MapUtilities.ExitEvents?.Invoke(ref mapData);
            ExitEvents();
        }
    }
}