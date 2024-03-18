using UnityEngine;

public class RoomEvents : MonoBehaviour
{
    public delegate void Enter(ref MapData mapData);
    public delegate void Exit(ref MapData mapData);

    MapData mapData = new MapData();
    public Enter enterEvents;
    public Exit exitEvents;

    private void Awake()
    {
        AddEnterEvent(TestFunction);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            enterEvents?.Invoke(ref mapData);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            exitEvents?.Invoke(ref mapData);
        }
    }

    public void TestFunction(ref MapData mapData)
    {
        Debug.Log("WOOOW");
    }

    public void AddEnterEvent(Enter enterEvent)
    {
        enterEvents += enterEvent;
    }

    public void AddExitEvent(Exit exitEvent)
    {
        exitEvents += exitEvent;
    }
}