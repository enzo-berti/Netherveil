using Map.Component;
using Map.Generation;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;

namespace Map
{
    public class RoomEvents : MonoBehaviour
    {
        private RoomData roomData;

        private Room room;
        private GameObject roomPreset;
        private GameObject enemies;
        private GameObject treasures;
        private NavMeshSurface navMeshSurface;

        static private bool hasLeaved = false;
        private bool allChestsOpenCalled = false;
        private bool allEnemiesDeadCalled = false;
        private bool enterRoomCalled = false;
        private bool exitRoomCalled = false;

        private Collider triggerCollide = null;
        private Vector3 enterPos = Vector3.zero;

        private void Awake()
        {
            foreach (Collider coll in GetComponents<Collider>())
            {
                if (coll.isTrigger)
                {
                    triggerCollide = coll;
                    break;
                }
            }

            // find room go's
            room = transform.parent.gameObject.GetComponent<Room>();
            room.GetComponentInChildren<RoomPresets>(true).GenerateRandomPreset(); // je détruis tout ici pour me simplifier la vie..
            roomPreset = room.GetComponentInChildren<RoomPresets>(true).transform.GetChild(0).gameObject;
            enemies = roomPreset.transform.Find("Enemies").gameObject;
            treasures = roomPreset.transform.Find("Treasures").gameObject;
            navMeshSurface = room.GetComponentInChildren<NavMeshSurface>(true);

            enemies.SetActive(false);
            foreach (var c in room.GetComponentsInChildren<MapLayer>(true))
            {
                c.Unset();
            }

            var roomUI = room.GetComponentInChildren<RoomUI>(true);
            if (roomUI)
            {
                roomUI.gameObject.SetActive(false);
            }

            // create data of the map
            roomData = new RoomData(room, enemies);
            if (roomData.Type == RoomType.Lobby) // because enter not called frame one in game (dumb fix)
            {
                EnterEvents();
            }
        }

        private void Start()
        {
            // set bool to true to not call the events in the room if there is no enemy
            allEnemiesDeadCalled = (enemies.transform.childCount == 0);
            // set bool to true to not call the events in the room if there is no chest
            allChestsOpenCalled = (treasures.GetComponentsInChildren<Item>().Count() == 0);
        }

        private void OpenAllChestsEvent()
        {
            allChestsOpenCalled = true;

            // global events
            MapUtilities.onEarlyAllChestOpen?.Invoke();
            MapUtilities.onAllChestOpen?.Invoke();
        }

        private void LocalEnterEvents()
        {
            enterRoomCalled = true;
            RoomEvents.hasLeaved = false;

            // set all elements to the map layer now that we can see them
            foreach (var c in room.GetComponentsInChildren<MapLayer>())
            {
                c.Set();
            }

            // activate ui
            RoomUI roomUI = room.GetComponentInChildren<RoomUI>(true);
            if (roomUI)
            {
                roomUI.gameObject.SetActive(true);
            }

            MapUtilities.currentRoomData = roomData;
            MapUtilities.nbEnterRoomByType[MapUtilities.currentRoomData.Type] += 1;

            navMeshSurface.enabled = true;
            enemies.SetActive(true);
        }

        private void EnterEvents()
        {
            LocalEnterEvents();

            // global events
            MapUtilities.onEarlyEnter?.Invoke();
            MapUtilities.onEnter?.Invoke();
            Debug.Log("ENTER ROOM");
        }

        private void LocalExitEvents()
        {
            exitRoomCalled = true;
            RoomEvents.hasLeaved = true;

            navMeshSurface.enabled = false;
            enemies.SetActive(false);
        }

        private void ExitEvents()
        {
            LocalExitEvents();

            // global events
            MapUtilities.onEarlyExit?.Invoke();
            MapUtilities.onExit?.Invoke();
            Debug.Log("EXIT ROOM");
        }

        private void AllEnemiesDeadEvents()
        {
            // local events
            allEnemiesDeadCalled = true;

            // global events
            MapUtilities.onEarlyAllEnemiesDead?.Invoke();
            MapUtilities.onAllEnemiesDead?.Invoke();

            for (int i = 0; i < transform.parent.parent.childCount; i++)
            {
                if (transform.parent.parent.GetChild(i) == transform.parent)
                {
                    FindObjectOfType<MapGenerator>().roomClearId.Add(i);
                    break;
                }
            }

            SaveManager.Instance.Save();
            Debug.Log("KILL ALL ENEMIES", gameObject);
        }

        private void LateUpdate()
        {
            if (!allEnemiesDeadCalled && enemies.transform.childCount == 0)
            {
                AllEnemiesDeadEvents();
            }

            if (!allChestsOpenCalled && treasures.GetComponentsInChildren<Item>().Count() == 0)
            {
                OpenAllChestsEvent();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!enterRoomCalled && other.gameObject.CompareTag("Player"))
            {
                enterPos = triggerCollide.ClosestPointOnBounds(other.bounds.center);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!enterRoomCalled && other.gameObject.CompareTag("Player") && hasLeaved)
            {
                Vector3 enterToPlayer = enterPos - other.bounds.center;
                if (enterToPlayer.magnitude >= 6.25f)
                {
                    EnterEvents();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!exitRoomCalled && other.gameObject.CompareTag("Player"))
            {
                ExitEvents();
            }
        }

        public void Clear()
        {
            LocalEnterEvents();
            LocalExitEvents();
            RoomEvents.hasLeaved = false; // to ensure not being blocked
        }
    }
}