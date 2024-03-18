using UnityEngine;

public class RoomEvents : MonoBehaviour
{
    public delegate void Enter(ref MapData mapData);
    public delegate void Exit(ref MapData mapData);

    MapData mapData = new MapData();
    public event Enter EnterEvents;
    public event Exit ExitEvents;

    private void Start()
    {
        // TODO : Create mapData here
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            EnterEvents?.Invoke(ref mapData);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ExitEvents?.Invoke(ref mapData);
        }
    }

    public void AddEnterEvent(Enter enterEvent)
    {
        EnterEvents += enterEvent;
    }

    public void AddExitEvent(Exit exitEvent)
    {
        ExitEvents += exitEvent;
    }
}