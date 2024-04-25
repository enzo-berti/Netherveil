using Map.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;

namespace Map.Generation
{
    public struct GenerationParam
    {
        public Dictionary<RoomType, int> nbRoomByType;
        public Dictionary<int, List<Door>> availableDoorsByRotation;

        public GenerationParam(int nbNormal = 0, int nbTreasure = 0, int nbChallenge = 0, int nbMerchant = 0, int nbSecret = 0, int nbMiniBoss = 0)
        {
            nbRoomByType = new Dictionary<RoomType, int>
            {
                { RoomType.Lobby, 0 },
                { RoomType.Normal, nbNormal },
                { RoomType.Treasure, nbTreasure },
                { RoomType.Challenge, nbChallenge },
                { RoomType.Merchant, nbMerchant },
                { RoomType.Secret, nbSecret },
                { RoomType.MiniBoss, nbMiniBoss },
                { RoomType.Boss, 0 },
            };

            availableDoorsByRotation = new Dictionary<int, List<Door>>
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

                foreach (var list in availableDoorsByRotation.Values)
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
                if (availableDoorsByRotation.ContainsKey(rotation))
                {
                    availableDoorsByRotation[rotation].Add(door);
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
            foreach (var doors in availableDoorsByRotation)
            {
                result.AddRange(doors.Value);
            }

            result.Sort((a, b) => (int)(b.Position.magnitude - a.Position.magnitude));
            return result;
        }

        public readonly List<Door> GetFarestDoorsByRot(int rot)
        {
            List<Door> result = availableDoorsByRotation[rot];

            result.Sort((a, b) => (int)(b.Position.magnitude - a.Position.magnitude));
            return result;
        }

        public readonly void RemoveDoor(Door door)
        {
            foreach (var doors in availableDoorsByRotation.Values)
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
        [SerializeField] private bool isRandom = true;
        [SerializeField] private string seed;

        [SerializeField] private List<GameObject> obstructionsDoor;
        [SerializeField] private List<GameObject> stairsPrefab;
        [SerializeField] private GameObject gatePrefab;

        private readonly List<int> availableRotations = new List<int>() { 0, 90, 180, 270 };

        private void Awake()
        {
            Seed.RandomizeSeed();
            if (!isRandom)
            {
                Seed.Set(seed);
            }

            GenerateMap(new GenerationParam(nbNormal: 8, nbTreasure: 2, nbMerchant: 1, nbSecret: 0));
        }

        private void GenerateMap(GenerationParam genParam)
        {
            RoomUtilities.nbRoomByType = genParam.nbRoomByType.ToDictionary(entry => entry.Key, entry => entry.Value);
            RoomUtilities.nbRoomByType[RoomType.Lobby] = 1;
            RoomUtilities.nbRoomByType[RoomType.Boss] = 1;

            GenerateLobbyRoom(ref genParam);
            GenerateRooms(ref genParam);

            // Generate boss rooms
            if (!GenerateRoom(ref genParam, RoomType.Boss))
            {
                Debug.LogError("Can't find any candidate for boss room");
            }

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
                    if (genParam.nbRoomByType[RoomType.Normal] <= 0)
                    {
                        Debug.LogError("No doors left to spawn another room");
                        return false;
                    }
                    else if (!GenerateRoom(ref genParam, RoomType.Normal))
                    {
                        Debug.LogError("Can't generate room");
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
                                Debug.LogError("Can't generate room");
                                return false;
                            }
                            genParam.nbRoomByType[type]--;
                            break;
                        }
                    }
                }
            }

            return true;
        }

        private bool GenerateRoom(ref GenerationParam genParam, RoomType type)
        {
            foreach (RoomPrefab candidateRoomPrefab in Seed.RandList(MapResources.RoomPrefabs(type)))
            {
                GameObject roomGO = Instantiate(candidateRoomPrefab.gameObject);
                if (!TryPutRoom(roomGO, ref genParam, out Door entranceDoor, out Door exitDoor))
                {
                    continue;
                }

                InitRoom(roomGO, ref genParam, entranceDoor, exitDoor);
                return true;
            }

            Debug.LogError("Can't find any candidate for map room");
            return false;
        }

        private void GenerateLobbyRoom(ref GenerationParam genParam)
        {
            GameObject roomGO = Instantiate(MapResources.RandPrefabRoom(RoomType.Lobby).gameObject);

            DoorsGenerator doorsGenerator = roomGO.GetComponentInChildren<DoorsGenerator>();

            genParam.AddAvailableDoors(doorsGenerator);
            Destroy(doorsGenerator);

            roomGO.transform.parent = gameObject.transform;
        }

        private bool TryPutRoom(GameObject roomGO, ref GenerationParam genParam, out Door entranceDoor, out Door exitDoor)
        {
            DoorsGenerator doorsGenerator = roomGO.GetComponentInChildren<DoorsGenerator>();

            foreach (Door entranceDoorCandidate in Seed.RandList(doorsGenerator.doors))
            {
                if (TrySetEntranceDoorPos(roomGO, ref genParam, entranceDoorCandidate, out exitDoor))
                {
                    entranceDoor = entranceDoorCandidate;
                    return true;
                }
            }

            entranceDoor = new Door();
            exitDoor = new Door();
            DestroyImmediate(roomGO);
            return false;
        }

        private bool TrySetEntranceDoorPos(GameObject roomGO, ref GenerationParam genParam, Door entranceDoor, out Door exitDoor)
        {
            foreach (int rotation in Seed.RandList(availableRotations))
            {
                if (!genParam.availableDoorsByRotation[rotation].Any())
                {
                    continue;
                }

                float defaultRot = roomGO.transform.rotation.eulerAngles.y;
                foreach (Door candidateExitDoor in Seed.RandList(genParam.availableDoorsByRotation[rotation]))
                {
                    // rotate gameObject entrance door to correspond the exit door
                    roomGO.transform.rotation = Quaternion.Euler(0f, (int)(rotation - 180f - entranceDoor.Rotation + defaultRot), 0f);

                    // Set position
                    roomGO.transform.position = entranceDoor.parentSkeleton.transform.position - entranceDoor.Position + candidateExitDoor.Position; // exit.pos = entrance.pos + (-entrance.arrow.pos + exit.arrow.pos) + forward * 0.1 (forward = offset)
                    Physics.SyncTransforms(); // need to update physics before doing collision test in the same frame

                    // Check collision
                    if (IsRoomCollidingOtherRoom(roomGO, candidateExitDoor))
                    {
                        roomGO.transform.rotation = Quaternion.Euler(0f, defaultRot, 0f); // reset rotation
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
            roomGO.transform.SetParent(gameObject.transform);

            // Generate gate
            GameObject gateGO = Instantiate(gatePrefab, entranceDoor.Position, Quaternion.identity);
            gateGO.transform.Rotate(0f, entranceDoor.Rotation, 0f);
            gateGO.transform.parent = roomGO.GetComponentInChildren<StaticProps>().transform;

            // Removed used door
            DoorsGenerator doorsGenerator = roomGO.GetComponentInChildren<DoorsGenerator>();
            doorsGenerator.RemoveDoor(entranceDoor);
            genParam.RemoveDoor(exitDoor);

            // Add the new doors from the new room into the possible candidates
            genParam.AddAvailableDoors(doorsGenerator);

            if (roomGO.GetComponent<RoomPrefab>().type == RoomType.Boss)
            {
                GenerateStairs(roomGO);
            }

            // Generate one of the seed room and delete the other's
            roomGO.GetComponentInChildren<RoomPresets>().GenerateRandomPreset();

            // SetActive object's of room
            roomGO.GetComponentInChildren<NavMeshSurface>().transform.Find("Enemies").gameObject.SetActive(false);
            roomGO.GetComponentInChildren<NavMeshSurface>().enabled = false;
        }

        static private bool IsRoomCollidingOtherRoom(GameObject roomGO, Door exitDoor)
        {
            GameObject skeletonGO = roomGO.GetComponentInChildren<Skeleton>().gameObject;
            BoxCollider roomColliderEnter = skeletonGO.GetComponent<BoxCollider>();
            BoxCollider roomColliderExit = exitDoor.parentSkeleton.GetComponent<BoxCollider>();

            // if we find another trigger with the "map" tag then we collide with another room
            Collider[] colliders = roomColliderEnter.BoxOverlap(LayerMask.GetMask("Map"), QueryTriggerInteraction.UseGlobal).Where(collider => collider != roomColliderEnter && collider != roomColliderExit && collider.isTrigger).ToArray();
            return colliders.Any();
        }

        private void ResetGeneration()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        private void GenerateObstructionDoors(ref GenerationParam genParam)
        {
            foreach (var listDoors in genParam.availableDoorsByRotation)
            {
                foreach (var door in listDoors.Value)
                {
                    GameObject go = Instantiate(Seed.RandList(obstructionsDoor)[0], door.Position, Quaternion.identity);
                    go.transform.Rotate(0f, door.Rotation, 0f);
                    go.transform.parent = door.parentSkeleton.transform.Find("StaticProps");
                }
            }

            genParam.availableDoorsByRotation.Clear();
        }

        private void GenerateStairs(GameObject roomGO)
        {
            DoorsGenerator doorsGenerator = roomGO.transform.Find("Skeleton").Find("Doors").GetComponent<DoorsGenerator>();
            Door entranceStairs = Seed.RandList(doorsGenerator.doors)[0];

            GameObject go = Instantiate(Seed.RandList(stairsPrefab)[0], entranceStairs.Position, Quaternion.identity);
            go.transform.Rotate(0f, entranceStairs.Rotation, 0f);
            go.transform.parent = entranceStairs.parentSkeleton.transform.Find("StaticProps");

            doorsGenerator.RemoveDoor(entranceStairs);
        }
    }
}