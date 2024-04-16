using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using Map;

namespace Generation
{
    public struct GenerationParam
    {
        public Dictionary<RoomType, int> nbRoomByType;
        public Dictionary<int, List<Door>> availableDoorsByRot;

        public GenerationParam(int nbNormal = 0, int nbTreasure = 0, int nbChallenge = 0, int nbMerchant = 0, int nbSecret = 0, int nbMiniBoss = 0)
        {
            nbRoomByType = new Dictionary<RoomType, int>
            {
                { RoomType.Normal, nbNormal },
                { RoomType.Treasure, nbTreasure },
                { RoomType.Challenge, nbChallenge },
                { RoomType.Merchant, nbMerchant },
                { RoomType.Secret, nbSecret },
                { RoomType.MiniBoss, nbMiniBoss },
            };

            availableDoorsByRot = new Dictionary<int, List<Door>>
            {
                { 0, new List<Door>() },
                { 90, new List<Door>() },
                { 180, new List<Door>() },
                { 270, new List<Door>() }
            };
        }

        public readonly int NbRoom
        {
            get
            {
                int totalCount = 0;
                foreach (int count in nbRoomByType.Values)
                {
                    totalCount += count;
                }

                return totalCount;
            }
        }

        public int AvailableDoorsCount
        {
            get
            {
                int count = 0;

                foreach (var list in availableDoorsByRot.Values)
                {
                    count += list.Count;
                }

                return count;
            }
        }

        public readonly void AddAvailableDoors(DoorsGenerator doorsGenerator)
        {
            foreach (var door in doorsGenerator.doors)
            {
                int rotation = (int)Math.Round(door.Rotation);
                if (availableDoorsByRot.ContainsKey(rotation))
                {
                    availableDoorsByRot[rotation].Add(door);
                }
                else
                {
                    Debug.LogError("Error try to insert an object with a not allowed rotation : " + door.Rotation, doorsGenerator);
                }
            }

            //UnityEngine.Object.Destroy(doorsGenerator); // destroy doorsGenerator
        }

        public readonly List<Door> GetFarestDoors()
        {
            List<Door> result = new List<Door>();
            foreach (var doors in availableDoorsByRot)
            {
                result.AddRange(doors.Value);
            }

            result.Sort((a, b) => (int)(b.Position.magnitude - a.Position.magnitude));
            return result;
        }

        public readonly List<Door> GetFarestDoorsByRot(int rot)
        {
            List<Door> result = availableDoorsByRot[rot];

            result.Sort((a, b) => (int)(b.Position.magnitude - a.Position.magnitude));
            return result;
        }

        public readonly void RemoveDoor(Door door)
        {
            foreach (var doors in availableDoorsByRot.Values)
            {
                if (doors.Remove(door))
                {
                    return;
                }
            }
        }
    }

    public class MapGenerator : MonoBehaviour
    {
        private static readonly List<int> availableRotations = new List<int>() { 0, 90, 180, 270 };

        [SerializeField] private bool isRandom = true;
        [SerializeField] private string seed;

        [SerializeField] private List<GameObject> roomLobby = new List<GameObject>();
        [SerializeField] private List<GameObject> roomNormal = new List<GameObject>();
        [SerializeField] private List<GameObject> roomTreasure = new List<GameObject>();
        [SerializeField] private List<GameObject> roomChallenge = new List<GameObject>();
        [SerializeField] private List<GameObject> roomMerchant = new List<GameObject>();
        [SerializeField] private List<GameObject> roomSecret = new List<GameObject>();
        [SerializeField] private List<GameObject> roomMiniBoss = new List<GameObject>();
        [SerializeField] private List<GameObject> roomBoss = new List<GameObject>();

        [SerializeField] private List<GameObject> obstructionsDoor;
        [SerializeField] private List<GameObject> stairsPrefab;
        [SerializeField] private GameObject gatePrefab;

        private void Awake()
        {
            Seed.RandomizeSeed();
            if (!isRandom)
            {
                Seed.Set(seed);
            }

            GenerateMap(new GenerationParam(nbNormal: 2, nbTreasure: 4, nbMerchant: 1));
        }

        private void GenerateMap(GenerationParam genParam)
        {
            GenerateLobbyRoom(ref genParam);
            GenerateRooms(ref genParam);
            GenerateObstructionDoors(ref genParam);
        }

        private bool GenerateRooms(ref GenerationParam genParam)
        {
            int nbRoom = genParam.NbRoom;

            for (int i = 0; i < nbRoom; i++)
            {
                // if not enough door are available, spawn a normal room by force
                if (genParam.AvailableDoorsCount <= 1)
                {
                    if (genParam.nbRoomByType[RoomType.Normal] <= 0 || !GenerateRoom(ref genParam, RoomType.Normal))
                    {
                        Debug.LogError("Can't find any valid room");
                        return false;
                    }
                    genParam.nbRoomByType[RoomType.Normal]--;
                }
                else
                {
                    // TODO : completly redo the order of type generation
                    for (int index = 0; index < genParam.nbRoomByType.Count; index++)
                    {
                        RoomType type = genParam.nbRoomByType.Keys.ElementAt(index);
                        if (genParam.nbRoomByType[type] > 0)
                        {
                            if (!GenerateRoom(ref genParam, type))
                            {
                                return false;
                            }
                            genParam.nbRoomByType[type]--;
                            break;
                        }
                    }
                }
            }

            if (!TryInstantiateRoom(GetRandRoomGO(RoomType.Boss), RoomType.Boss, ref genParam))
            {
                Debug.LogError("Can't find any candidate for boss room");
                return false;
            }

            return true;
        }

        private bool GenerateRoom(ref GenerationParam genParam, RoomType type)
        {
            foreach (GameObject candidatePrefab in Seed.RandList(GetRoomsGO(type)))
            {
                if (TryInstantiateRoom(candidatePrefab, type, ref genParam))
                {
                    return true;
                }
            }

            Debug.LogError("Can't find any candidate for map room");
            return false;
        }

        private void GenerateLobbyRoom(ref GenerationParam genParam)
        {
            GameObject roomGO = Instantiate(GetRandRoomGO(RoomType.Lobby));
            roomGO.GetComponentInChildren<RoomGenerator>().type = RoomType.Lobby;

            DoorsGenerator doorsGenerator = roomGO.transform.Find("Skeleton").Find("Doors").GetComponent<DoorsGenerator>();

            genParam.AddAvailableDoors(doorsGenerator);
            Destroy(doorsGenerator);

            roomGO.transform.parent = gameObject.transform;
        }

        private void GenerateObstructionDoors(ref GenerationParam genParam)
        {
            foreach (var listDoors in genParam.availableDoorsByRot)
            {
                foreach (var door in listDoors.Value)
                {
                    GameObject go = Instantiate(obstructionsDoor[UnityEngine.Random.Range(0, obstructionsDoor.Count)], door.Position, Quaternion.identity);
                    go.transform.Rotate(0, door.Rotation, 0);
                    go.transform.parent = door.parentSkeleton.transform.Find("StaticProps");
                }
            }

            genParam.availableDoorsByRot.Clear();
        }

        private bool TryInstantiateRoom(GameObject prefabRoom, RoomType type, ref GenerationParam genParam)
        {
            GameObject roomGO = Instantiate(prefabRoom);
            roomGO.GetComponentInChildren<RoomGenerator>().type = type;

            DoorsGenerator doorsGenerator = roomGO.transform.Find("Skeleton").Find("Doors").GetComponent<DoorsGenerator>();

            foreach (Door entranceDoor in Seed.RandList(doorsGenerator.doors))
            {
                if (TrySetEntranceDoorPos(roomGO, ref genParam, entranceDoor, out Door exitDoor))
                {
                    InitRoom(roomGO, ref genParam, entranceDoor, exitDoor);
                    return true;
                }
            }

            DestroyImmediate(roomGO); // didn't find any spawn for this candidate
            return false;
        }

        private bool TrySetEntranceDoorPos(GameObject roomGO, ref GenerationParam genParam, Door entranceDoor, out Door exitDoor)
        {
            DoorsGenerator doorsGenerator = roomGO.transform.Find("Skeleton").Find("Doors").GetComponent<DoorsGenerator>();

            foreach (int rotation in Seed.RandList(availableRotations))
            {
                if (!genParam.availableDoorsByRot[rotation].Any())
                {
                    continue;
                }

                foreach (Door candidateExitDoor in Seed.RandList(genParam.availableDoorsByRot[rotation]))
                {
                    // rotate gameObject entrance door to correspond the exit door
                    doorsGenerator.transform.parent.parent.rotation = Quaternion.Euler(0f, (int)(rotation - 180f - entranceDoor.Rotation), 0f);

                    // Set position
                    roomGO.transform.position = entranceDoor.parentSkeleton.transform.parent.transform.position - entranceDoor.Position + candidateExitDoor.Position; // exit.pos = entrance.pos + (-entrance.arrow.pos + exit.arrow.pos) + forward * 0.1 (forward = offset)
                    Physics.SyncTransforms(); // need to update physics before doing collision test in the same frame

                    // Check collision
                    if (IsRoomCollidingOtherRoom(roomGO, candidateExitDoor))
                    {
                        doorsGenerator.transform.parent.parent.rotation = Quaternion.Euler(0f, 0f, 0f); // reset rotation
                        continue; // fail to generate continue to next door candidate
                    }

                    exitDoor = candidateExitDoor;
                    return true;
                }
            }

            exitDoor = new Door();
            return false;
        }

        private void InitRoom(GameObject roomGO, ref GenerationParam genParam, Door entranceDoor, Door exitDoor)
        {
            // Set parent room
            roomGO.transform.parent = gameObject.transform;

            // Generate gate
            GameObject gateGO = Instantiate(gatePrefab, entranceDoor.Position, Quaternion.identity);
            gateGO.transform.Rotate(0f, entranceDoor.Rotation, 0f);
            gateGO.transform.parent = roomGO.transform.Find("Skeleton").Find("StaticProps");

            // Removed used door
            DoorsGenerator doorsGenerator = roomGO.transform.Find("Skeleton").Find("Doors").GetComponent<DoorsGenerator>();
            doorsGenerator.RemoveDoor(entranceDoor);
            genParam.RemoveDoor(exitDoor);

            // Add the new doors from the new room into the possible candidates
            genParam.AddAvailableDoors(doorsGenerator);

            // Generate one of the seed room and delete the other's
            roomGO.GetComponentInChildren<RoomGenerator>().GenerateRoomSeed();

            // SetActive object's of room
            roomGO.transform.Find("RoomGenerator").GetChild(0).Find("Enemies").gameObject.SetActive(false);
            roomGO.GetComponentInChildren<NavMeshSurface>().enabled = false;
        }

        static private bool IsRoomCollidingOtherRoom(GameObject roomGO, Door exitDoor)
        {
            BoxCollider roomColliderEnter = roomGO.transform.Find("Skeleton").GetComponent<BoxCollider>();
            BoxCollider roomColliderExit = exitDoor.parentSkeleton.GetComponent<BoxCollider>();

            // if we find another trigger with the "map" tag then we collide with another room
            Collider[] colliders = roomColliderEnter.BoxOverlap(LayerMask.GetMask("Map"), QueryTriggerInteraction.UseGlobal).Where(collider => collider != roomColliderEnter && collider != roomColliderExit && collider.isTrigger).ToArray();
            return colliders.Any();
        }

        private List<GameObject> GetRoomsGO(RoomType type)
        {
            return type switch // define type of list
            {
                RoomType.Lobby => roomLobby,
                RoomType.Normal => roomNormal,
                RoomType.Treasure => roomTreasure,
                RoomType.Challenge => roomChallenge,
                RoomType.Merchant => roomMerchant,
                RoomType.Secret => roomSecret,
                RoomType.MiniBoss => roomMiniBoss,
                RoomType.Boss => roomBoss,
                _ => null,
            };
        }

        private GameObject GetRandRoomGO(RoomType type)
        {
            List<GameObject> list = GetRoomsGO(type);

            if (list == null || !list.Any())
            {
                Debug.LogWarning("Can't find candidate room for type : " + type, this);
                return null;
            }

            int randIndex = Seed.Range(0, list.Count);
            return list[randIndex];
        }

        private void ResetGeneration()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }
    }
}