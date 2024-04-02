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
    Boss
}

public struct GenerationParam
{
    public Dictionary<RoomType, int> nbRoom;
    public Dictionary<float, List<Door>> availableDoors;


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

        availableDoors = new Dictionary<float, List<Door>>
        {
            { 0f, new List<Door>() },
            { 90f, new List<Door>() },
            { 180f, new List<Door>() },
            { 270f, new List<Door>() }
        };
    }

    public readonly int TotalRoom
    {
        get
        {
            return nbRoom[RoomType.Normal] + nbRoom[RoomType.Treasure] + nbRoom[RoomType.Challenge] + nbRoom[RoomType.Merchant] + nbRoom[RoomType.Secret] + nbRoom[RoomType.MiniBoss];
        }
    }

    public int NumRoomAvaibles
    {
        get
        {
            int count = 0;

            foreach (var list in availableDoors.Values)
            {
                count += list.Count;
            }

            if (count == 0)
            {
                Debug.Log(availableDoors.Values.Count);
                foreach (var truc in availableDoors)
                {
                    Debug.Log("LIST : " + truc.Key + " NB : " + truc.Value.Count);
                }
            }

            return count;
        }
    }

    public readonly void AddDoorsGenerator(DoorsGenerator doorsGenerator)
    {
        foreach (var door in doorsGenerator.doors)
        {
            if (availableDoors.ContainsKey(door.Rotation))
            {
                availableDoors[door.Rotation].Add(door);
            }
            else
            {
                Debug.LogError("Error try to insert an object with a not allowed rotation : " + door.Rotation);
            }
        }

        UnityEngine.Object.Destroy(doorsGenerator); // destroy doorsGenerator
    }

    public readonly Door GetFarestDoor()
    {
        Tuple<int, float> farestDoor = new Tuple<int, float>(-1, 0);
        float key = 0f;

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
    public static int NoiseGenerator = 0;

    [SerializeField] private List<GameObject> roomLobby = new List<GameObject>();
    [SerializeField] private List<GameObject> roomNormal = new List<GameObject>();
    [SerializeField] private List<GameObject> roomTreasure = new List<GameObject>();
    [SerializeField] private List<GameObject> roomChallenge = new List<GameObject>();
    [SerializeField] private List<GameObject> roomMerchant = new List<GameObject>();
    [SerializeField] private List<GameObject> roomSecret = new List<GameObject>();
    [SerializeField] private List<GameObject> roomMiniBoss = new List<GameObject>();
    [SerializeField] private List<GameObject> roomBoss = new List<GameObject>();

    [SerializeField] private List<GameObject> obstructionsDoor;
    [SerializeField] private GameObject gate;

    [SerializeField] int nbNormalRoom;

    private void Awake()
    {
        GenerateMap(new GenerationParam(nbNormal: nbNormalRoom));
    }

    private void GenerateMap(GenerationParam genParam)
    {
        int nbRoom = genParam.TotalRoom;
        GenerateLobbyRoom(ref genParam);

        for (int i = 0; i < nbRoom - 1; i++)
        {
            GenerateRoom(ref genParam);
        }

        GenerateBossRoom(ref genParam);

        // TODO : spawn things to hides the holes
        foreach (var listDoors in genParam.availableDoors)
        {
            foreach (var door in listDoors.Value)
            {
                GameObject go = Instantiate(obstructionsDoor[UnityEngine.Random.Range(0, obstructionsDoor.Count)], door.Position, Quaternion.identity);
                go.transform.Rotate(0, door.Rotation, 0);
                go.transform.parent = gameObject.transform;
            }
        }
    }

    bool GetDoorCandidates(ref GenerationParam genParam, DoorsGenerator doorsGenerator, out Door entranceDoor, out Door exitDoor)
    {
        entranceDoor = new Door();
        exitDoor = new Door();

        int noiseIndex = GameAssets.Instance.seed.Range(0, doorsGenerator.doors.Count, ref NoiseGenerator);
        for (int i = 0; noiseIndex < doorsGenerator.doors.Count; i++)
        {
            int index = (i + noiseIndex) % doorsGenerator.doors.Count;
            Door door = doorsGenerator.doors[index];

            for (int j = 0; j < 4; j++)
            {
                float indexRotation = (noiseIndex + j) % 4;
                float doorRotation = door.Rotation + 90f * indexRotation;
                float neededRotation = (doorRotation + 180f) % 360f;

                if (genParam.availableDoors.ContainsKey(neededRotation) && genParam.availableDoors[neededRotation].Count != 0)
                {
                    int randIndex = GameAssets.Instance.seed.Range(0, genParam.availableDoors[neededRotation].Count, ref NoiseGenerator);

                    entranceDoor = door;
                    exitDoor = genParam.availableDoors[neededRotation][randIndex];

                    doorsGenerator.transform.parent.parent.Rotate(0, 90f * indexRotation, 0); // rotate gameObject entrance to correspond the neededRotation

                    return true;
                }
            }
        }

        return false;
    }

    void GenerateRoom(ref GenerationParam genParam)
    {
        bool hasGenerated = false;
        while (!hasGenerated)
        {
            // instantiate room with first availableDoors transform then remove it
            int prefabIndex = GameAssets.Instance.seed.Range(0, roomNormal.Count, ref NoiseGenerator);
            GameObject roomGO = Instantiate(roomNormal[prefabIndex]); // TODO : add random selection
            roomGO.GetComponentInChildren<RoomGenerator>().type = RoomType.Normal;

            DoorsGenerator doorsGenerator = roomGO.transform.Find("Skeleton").transform.Find("Doors").GetComponent<DoorsGenerator>();
            doorsGenerator.GenerateSeed(genParam);

            if (!GetDoorCandidates(ref genParam, doorsGenerator, out Door entranceDoor, out Door exitDoor))
            {
                DestroyImmediate(roomGO);
                continue;
            }

            if (!InitiateGoRoom(roomGO, ref genParam, entranceDoor, exitDoor))
            {
                continue; // fail to generate continue to next candidate
            }

            genParam.nbRoom[RoomType.Normal] -= doorsGenerator.doors.Count;
            hasGenerated = true;
        }
    }

    private void GenerateLobbyRoom(ref GenerationParam genParam)
    {
        GameObject roomGO = Instantiate(roomLobby[GameAssets.Instance.seed.Range(0, roomLobby.Count, ref NoiseGenerator)]);

        roomGO.GetComponentInChildren<RoomGenerator>().type = RoomType.Lobby;

        DoorsGenerator doorsGenerator = roomGO.transform.Find("Skeleton").transform.Find("Doors").GetComponent<DoorsGenerator>();
        doorsGenerator.GenerateSeed(genParam);

        genParam.AddDoorsGenerator(doorsGenerator);
        Destroy(doorsGenerator);

        genParam.nbRoom[RoomType.Normal] -= doorsGenerator.doors.Count;
        roomGO.transform.parent = gameObject.transform;
    }

    private void GenerateBossRoom(ref GenerationParam genParam)
    {
        foreach (Door exitDoor in genParam.GetFarestDoors())
        {
            GameObject roomBossGO = Instantiate(roomBoss[0]);
            DoorsGenerator doorsGenerator = roomBossGO.transform.Find("Skeleton").transform.Find("Doors").GetComponent<DoorsGenerator>();
            roomBossGO.GetComponentInChildren<RoomGenerator>().type = RoomType.Boss;

            Door entranceDoor = new Door();
            for (int i = 0; 0 < doorsGenerator.doors.Count; i++)
            {
                entranceDoor = doorsGenerator.doors[i];

                if (((entranceDoor.Rotation + 180f) % 360f) == exitDoor.Rotation)
                {
                    break;
                }
            }

            if (InitiateGoRoom(roomBossGO, ref genParam, entranceDoor, exitDoor))
            {
                continue; // fail to generate continue to next candidate
            }

            break;
        }
    }

    private bool InitiateGoRoom(GameObject roomGO, ref GenerationParam genParam, Door entranceDoor, Door exitDoor)
    {
        // Set position
        roomGO.transform.position = entranceDoor.parentSkeleton.transform.parent.transform.position - entranceDoor.Position + exitDoor.Position; // exit.pos = entrance.pos + (-entrance.arrow.pos + exit.arrow.pos) + forward * 0.1 (forward = offset)
        Physics.SyncTransforms(); // need to update physics before doing collision test in the same frame (bad)

        // Check collision
        if (IsRoomCollidingOtherRoom(roomGO, exitDoor))
        {
            DestroyImmediate(roomGO);
            return false;
        }

        // Generate gate
        GameObject gateGO = Instantiate(gate, entranceDoor.Position, Quaternion.identity);
        gateGO.transform.Rotate(0, entranceDoor.Rotation, 0);
        gateGO.transform.parent = gameObject.transform;

        // Removed used door
        DoorsGenerator doorsGenerator = roomGO.transform.Find("Skeleton").transform.Find("Doors").GetComponent<DoorsGenerator>();
        doorsGenerator.RemoveDoor(entranceDoor);
        genParam.RemoveDoor(exitDoor);

        // Add the new doors from the new room into the possible candidates
        genParam.AddDoorsGenerator(doorsGenerator);

        // Generate one of the seed room and delete the other's
        roomGO.GetComponentInChildren<RoomGenerator>().GenerateRoomSeed();

        // SetActive object's of room
        roomGO.transform.Find("RoomGenerator").GetChild(0).Find("Enemies").gameObject.SetActive(false);
        roomGO.GetComponentInChildren<NavMeshSurface>().enabled = false;

        // Set parent go
        roomGO.transform.parent = gameObject.transform;

        return true;
    }

    static private bool IsRoomCollidingOtherRoom(GameObject roomGO, Door exitDoor)
    {
        BoxCollider roomCollider = roomGO.transform.Find("Skeleton").GetComponent<BoxCollider>();
        BoxCollider roomColliderExit = exitDoor.parentSkeleton.GetComponent<BoxCollider>();

        Collider[] colliders = roomCollider.BoxOverlap(LayerMask.GetMask("Map"), QueryTriggerInteraction.Collide).Where(collider => collider != roomCollider && collider != roomColliderExit).ToArray();

        return colliders.Length > 2; // more than the two meshCollider
    }
}
