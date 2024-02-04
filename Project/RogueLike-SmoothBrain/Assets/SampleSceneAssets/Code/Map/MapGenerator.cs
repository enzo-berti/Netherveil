using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenerationParameters
{
    public int nbNormal;
    public int nbTreasure;
    public int nbChallenge;
    public int nbMerchant;
    public int nbSecret;
    public int nbMiniBoss;
    public int nbRoomMiniBoss;
    public int nbRoomBoss;

    public int NbRoom
    {
        get { return nbNormal + nbTreasure + nbChallenge + nbMerchant + nbSecret + nbMiniBoss + nbRoomMiniBoss + nbRoomBoss; }
    }
}

public class MapGenerator : MonoBehaviour
{
    public static int NoiseGenerator = 0;

    [SerializeField] private List<GameObject> roomNormal = new List<GameObject>();
    [SerializeField] private List<GameObject> roomTreasure = new List<GameObject>();
    [SerializeField] private List<GameObject> roomChallenge = new List<GameObject>();
    [SerializeField] private List<GameObject> roomMerchant = new List<GameObject>();
    [SerializeField] private List<GameObject> roomSecret = new List<GameObject>();
    [SerializeField] private List<GameObject> roomMiniBoss = new List<GameObject>();
    [SerializeField] private List<GameObject> roomBoss = new List<GameObject>();

    // will be moved in function directly
    private readonly Dictionary<float, List<Door>> availableDoors = new Dictionary<float, List<Door>>();
    private GenerationParameters generationParameters;

    private MapGenerator()
    {
        availableDoors.Add(0f, new List<Door>());
        availableDoors.Add(90f, new List<Door>());
        availableDoors.Add(180f, new List<Door>());
        availableDoors.Add(270f, new List<Door>());
    }

    private void Start()
    {
        GameManager.Instance.seed.Set(39423823219);
        generationParameters = new GenerationParameters();
        generationParameters.nbNormal = 100;

        //GenerateMap(generationParameters);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            GenerateRoom();
        }
    }

    bool GetCandidates(DoorsGenerator doorsGenerator, out Door entranceDoor, out Door exitDoor)
    {
        entranceDoor = new Door();
        exitDoor = new Door();

        int loopCount = 0;
        for (int startIndex = GameManager.Instance.seed.Range(0, doorsGenerator.doors.Count, ref NoiseGenerator); loopCount < doorsGenerator.doors.Count; loopCount++)
        {
            Door door = doorsGenerator.doors[(startIndex + loopCount) % doorsGenerator.doors.Count];
            float neededRotation = (door.rotation + 180f) % 360f;

            if (availableDoors.ContainsKey(neededRotation) && availableDoors[neededRotation].Count != 0)
            {
                int randIndex = GameManager.Instance.seed.Range(0, availableDoors[neededRotation].Count, ref NoiseGenerator);

                entranceDoor = door;
                exitDoor = availableDoors[neededRotation][randIndex];
                break;
            }
        }

        if (loopCount == doorsGenerator.doors.Count) // couldn't find a candidate
        {
            return false;
        }

        return true;
    }

    void GenerateRoom()
    {
        GameObject roomGO;
        if (availableDoors.CountValues() != 0)
        {
            // instantiate room with first availableDoors transform then remove it
            roomGO = Instantiate(roomNormal[GameManager.Instance.seed.Range(0, roomNormal.Count, ref NoiseGenerator)]); // TODO : add random selection

            DoorsGenerator doorsGenerator = roomGO.transform.Find("Skeleton").transform.Find("Instances_0").GetComponent<DoorsGenerator>();
            doorsGenerator.GenerateSeed(generationParameters);

            if (!GetCandidates(doorsGenerator, out Door entranceDoor, out Door exitDoor))
            {
                Destroy(roomGO);
                roomGO = null;
                return;
            }

            // sortie.pos = entree.pos + (-entree.arrow.pos + sortie.arrow.pos) + forward * 0.1 (forward = pour avoir un offset)
            roomGO.transform.position = entranceDoor.parentSkeleton.transform.parent.transform.position - entranceDoor.position + exitDoor.position + (-exitDoor.forward * 0.02f);
            Physics.SyncTransforms(); // need to update physics before doing testing in the same frame (bad)

            // bon sinon j'évite la collide de la salle et la salle exit (forcément que les deux collides putaig)
            BoxCollider roomCollider = roomGO.transform.Find("Skeleton").GetComponentInChildren<BoxCollider>();
            BoxCollider roomColliderExit = exitDoor.parentSkeleton.GetComponentInChildren<BoxCollider>();

            Collider[] colliders = roomCollider.BoxOverlap(LayerMask.GetMask("Map"), QueryTriggerInteraction.Collide).Where(collider => (collider != roomCollider && collider != roomColliderExit)).ToArray();
            if (colliders.Length != 0)
            {
                availableDoors[exitDoor.rotation].Remove(exitDoor);
                DestroyImmediate(roomGO);
                //DestroyImmediate(exitDoor);
                //i--; // generation failed then continue
                return;
            }

            // Close door for the next generation
            doorsGenerator.CloseDoor(entranceDoor);

            // Destroy used door
            availableDoors[exitDoor.rotation].Remove(exitDoor);

            // Add the new doors from the new room into the possible candidates
            foreach (var door in doorsGenerator.doors)
            {
                if (availableDoors.ContainsKey(door.rotation))
                {
                    availableDoors[door.rotation].Add(door);
                }
                else
                {
                    Debug.LogError("Error try to insert an object with a not allowed rotation : " + door.rotation);
                }
            }

            generationParameters.nbNormal -= doorsGenerator.doors.Count;
        }
        else // first room
        {
            roomGO = Instantiate(roomNormal[0]);

            DoorsGenerator doorsGenerator = roomGO.transform.Find("Skeleton").transform.Find("Instances_0").GetComponent<DoorsGenerator>();
            doorsGenerator.GenerateSeed(generationParameters);

            foreach (var door in doorsGenerator.doors)
            {
                if (availableDoors.ContainsKey(door.rotation))
                {
                    availableDoors[door.rotation].Add(door);
                }
                else
                {
                    Debug.LogError("Error try to insert an object with a not allowed rotation : " + door.rotation);
                }
            }

            generationParameters.nbNormal -= doorsGenerator.doors.Count;
        }

        roomGO.GetComponentInChildren<RoomGenerator>().GenerateRoomSeed();
        RoomGenerator.RoomGenerated++;
    }
}
