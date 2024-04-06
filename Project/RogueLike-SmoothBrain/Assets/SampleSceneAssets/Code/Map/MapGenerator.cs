using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;

public enum RoomType
{
    None,
    Lobby,
    Normal,
    Treasure,
    Challenge,
    Merchant,
    Secret,
    MiniBoss,
    Boss,

    COUNT
}

public struct GenerationParam
{
    public Dictionary<RoomType, int> nbRoom;
    public Dictionary<int, List<Door>> availableDoors;

    public GenerationParam(int nbNormal = 0, int nbTreasure = 0, int nbChallenge = 0, int nbMerchant = 0, int nbSecret = 0, int nbMiniBoss = 0)
    {

        nbRoom = new Dictionary<RoomType, int>
        {
            { RoomType.Normal, nbNormal },
            { RoomType.Treasure, nbTreasure },
            { RoomType.Challenge, nbChallenge },
            { RoomType.Merchant, nbMerchant },
            { RoomType.Secret, nbSecret },
            { RoomType.MiniBoss, nbMiniBoss },
        };

        availableDoors = new Dictionary<int, List<Door>>
        {
            { 0, new List<Door>() },
            { 90, new List<Door>() },
            { 180, new List<Door>() },
            { 270, new List<Door>() }
        };
    }

    public readonly int TotalRoom
    {
        get
        {
            int totalCount = 0;
            foreach(int count in nbRoom.Values)
            {
                totalCount += count;
            }

            return totalCount;
        }
    }

    public int RoomAvailablesCount
    {
        get
        {
            int count = 0;

            foreach (var list in availableDoors.Values)
            {
                count += list.Count;
            }

            return count;
        }
    }

    public readonly void AddDoorsGenerator(DoorsGenerator doorsGenerator)
    {
        foreach (var door in doorsGenerator.doors)
        {
            int rotation = (int)Math.Round(door.Rotation);
            if (availableDoors.ContainsKey(rotation))
            {
                availableDoors[rotation].Add(door);
            }
            else
            {
                Debug.LogError("Error try to insert an object with a not allowed rotation : " + door.Rotation, doorsGenerator);
            }
        }

        //UnityEngine.Object.Destroy(doorsGenerator); // destroy doorsGenerator
    }

    public readonly Door GetFarestDoor()
    {
        Tuple<int, float> farestDoor = new Tuple<int, float>(-1, 0);
        int key = 0;

        foreach (var doors in availableDoors)
        {
            for (int i = 0; i < doors.Value.Count; i++)
            {
                float distance = doors.Value[i].Position.magnitude;
                if (distance > farestDoor.Item2)
                {
                    key = doors.Key;
                    farestDoor = new Tuple<int, float>(i, distance);
                }
            }
        }

        return availableDoors[key][farestDoor.Item1];
    }

    public readonly List<Door> GetFarestDoors()
    {
        List<Door> result = new List<Door>();
        foreach (var doors in availableDoors)
        {
            result.AddRange(doors.Value);
        }

        result.Sort((a, b) => (int)(b.Position.magnitude - a.Position.magnitude));
        return result;
    }

    public readonly void RemoveDoor(Door door)
    {
        foreach (var doors in availableDoors.Values)
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

    [SerializeField] private List<GameObject> roomLobby = new List<GameObject>();
    [SerializeField] private List<GameObject> roomNormal = new List<GameObject>();
    [SerializeField] private List<GameObject> roomTreasure = new List<GameObject>();
    [SerializeField] private List<GameObject> roomChallenge = new List<GameObject>();
    [SerializeField] private List<GameObject> roomMerchant = new List<GameObject>();
    [SerializeField] private List<GameObject> roomSecret = new List<GameObject>();
    [SerializeField] private List<GameObject> roomMiniBoss = new List<GameObject>();
    [SerializeField] private List<GameObject> roomBoss = new List<GameObject>();

    [SerializeField] private List<GameObject> obstructionsDoor;
    [SerializeField] private GameObject gatePrefab;

    [SerializeField] private bool isRandom = true;
    [SerializeField] private int seed = 0;

    private void Awake()
    {
        if (!isRandom)
        {
            Seed.SetSeed(seed);
        }

        GenerateMap(new GenerationParam(nbNormal: 8, nbTreasure: 2));
    }

    private void GenerateMap(GenerationParam genParam)
    {
        GenerateLobbyRoom(ref genParam);

        GenerateRooms(ref genParam);

        GenerateBossRoom(ref genParam);

        GenerateObstructionDoors(ref genParam);
    }

    private void GenerateRooms(ref GenerationParam genParam)
    {
        int nbRoom = genParam.TotalRoom;

        for (int i = 0; i < nbRoom; i++)
        {
            // if not enough door are available, spawn a normal room by force
            if (genParam.RoomAvailablesCount <= 1)
            {
                if (!GenerateRoom(ref genParam, RoomType.Normal))
                {
                    return;
                }
                genParam.nbRoom[RoomType.Normal]--;
            }
            else
            {
                for (int index = 0; index < genParam.nbRoom.Count; index++)
                {
                    RoomType type = genParam.nbRoom.Keys.ElementAt(index);
                    if (genParam.nbRoom[type] > 0)
                    {
                        if (!GenerateRoom(ref genParam, type))
                        {
                            return;
                        }
                        genParam.nbRoom[type]--;
                        break;
                    }
                }
                // pas censer arriver jusqu'ici mdr
            }
        }
    }

    private bool GenerateRoom(ref GenerationParam genParam, RoomType type)
    {
        foreach (GameObject roomCandidate in Seed.RandList(GetRoomsGO(type)))
        {
            GameObject roomGO = Instantiate(roomCandidate);
            roomGO.GetComponentInChildren<RoomGenerator>().type = type;

            DoorsGenerator doorsGenerator = roomGO.transform.Find("Skeleton").Find("Doors").GetComponent<DoorsGenerator>();
            doorsGenerator.GenerateSeed(genParam);

            foreach (Door entranceDoor in doorsGenerator.RandomDoors)
            {
                foreach (int rotation in Seed.RandList(availableRotations))
                {
                    if (!genParam.availableDoors[rotation].Any())
                    {
                        continue;
                    }

                    foreach(Door exitDoor in Seed.RandList(genParam.availableDoors[rotation]))
                    {
                        doorsGenerator.transform.parent.parent.rotation = Quaternion.Euler(0f, (int)(rotation - 180f - entranceDoor.Rotation), 0f); // rotate gameObject exit to correspond the neededRotation

                        if (!TryInstantiateRoom(roomGO, ref genParam, entranceDoor, exitDoor))
                        {
                            doorsGenerator.transform.parent.parent.rotation = Quaternion.Euler(0f, 0f, 0f); // reset rotation
                            continue; // fail to generate continue to next candidate
                        }

                        return true;
                    }
                }
            }

            DestroyImmediate(roomGO); // didn't find any spawn for this candidate
        }

        Debug.LogError("PUTAIN ELLE CHARGE PAS CETTE SALOPE DE MAP");
        return false;
    }

    private void GenerateLobbyRoom(ref GenerationParam genParam)
    {
        GameObject roomGO = Instantiate(GetRandRoomGO(RoomType.Lobby));
        roomGO.GetComponentInChildren<RoomGenerator>().type = RoomType.Lobby;

        DoorsGenerator doorsGenerator = roomGO.transform.Find("Skeleton").Find("Doors").GetComponent<DoorsGenerator>();
        doorsGenerator.GenerateSeed(genParam);

        genParam.AddDoorsGenerator(doorsGenerator);
        Destroy(doorsGenerator);

        roomGO.transform.parent = gameObject.transform;
    }

    private void GenerateBossRoom(ref GenerationParam genParam)
    {
        foreach (Door exitDoor in genParam.GetFarestDoors())
        {
            GameObject roomBossGO = Instantiate(GetRandRoomGO(RoomType.Boss));
            roomBossGO.GetComponentInChildren<RoomGenerator>().type = RoomType.Boss;

            DoorsGenerator doorsGenerator = roomBossGO.transform.Find("Skeleton").Find("Doors").GetComponent<DoorsGenerator>();
            doorsGenerator.GenerateSeed(genParam);

            Door entranceDoor = new Door();
            for (int i = 0; i < doorsGenerator.doors.Count; i++)
            {
                entranceDoor = doorsGenerator.doors[i];

                if (((entranceDoor.Rotation + 180f) % 360f) == exitDoor.Rotation)
                {
                    break;
                }
            }

            if (!TryInstantiateRoom(roomBossGO, ref genParam, entranceDoor, exitDoor))
            {
                DestroyImmediate(roomBossGO);
                continue; // fail to generate continue to next candidate
            }

            return;
        }

        Debug.LogError("Can't find any candidate for boss room");
    }

    private void GenerateObstructionDoors(ref GenerationParam genParam)
    {
        foreach (var listDoors in genParam.availableDoors)
        {
            foreach (var door in listDoors.Value)
            {
                GameObject go = Instantiate(obstructionsDoor[UnityEngine.Random.Range(0, obstructionsDoor.Count)], door.Position, Quaternion.identity);
                go.transform.Rotate(0, door.Rotation, 0);
                go.transform.parent = door.parentSkeleton.transform.Find("Skeleton").Find("StaticProps");
            }
        }

        genParam.availableDoors.Clear();
    }

    private bool TryInstantiateRoom(GameObject roomGO, ref GenerationParam genParam, Door entranceDoor, Door exitDoor)
    {
        // Set position
        roomGO.transform.position = entranceDoor.parentSkeleton.transform.parent.transform.position - entranceDoor.Position + exitDoor.Position; // exit.pos = entrance.pos + (-entrance.arrow.pos + exit.arrow.pos) + forward * 0.1 (forward = offset)
        Physics.SyncTransforms(); // need to update physics before doing collision test in the same frame (bad)

        // Check collision
        if (IsRoomCollidingOtherRoom(roomGO, exitDoor))
        {
            return false;
        }

        // Set parent room
        roomGO.transform.parent = gameObject.transform;

        // Generate gate
        GameObject gateGO = Instantiate(gatePrefab, entranceDoor.Position, Quaternion.identity);
        gateGO.transform.Rotate(0, entranceDoor.Rotation, 0);
        gateGO.transform.parent = roomGO.transform.Find("Skeleton").Find("StaticProps");

        // Removed used door
        DoorsGenerator doorsGenerator = roomGO.transform.Find("Skeleton").Find("Doors").GetComponent<DoorsGenerator>();
        doorsGenerator.RemoveDoor(entranceDoor);
        genParam.RemoveDoor(exitDoor);

        // Add the new doors from the new room into the possible candidates
        genParam.AddDoorsGenerator(doorsGenerator);

        // Generate one of the seed room and delete the other's
        roomGO.GetComponentInChildren<RoomGenerator>().GenerateRoomSeed();

        // SetActive object's of room
        roomGO.transform.Find("RoomGenerator").GetChild(0).Find("Enemies").gameObject.SetActive(false);
        roomGO.GetComponentInChildren<NavMeshSurface>().enabled = false;

        return true;
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
}
