using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

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

    List<GameObject> availableDoors = new List<GameObject>();
    GenerationParameters generationParameters;

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

    void GenerateRoom()
    {
        GameObject roomGO;
        if (availableDoors.Count != 0)
        {
            int indexArrow = Random.Range(0, availableDoors.Count);
            // instantiate room with first availableDoors transform then remove it
            int oui = GameManager.Instance.seed.Range(0, roomNormal.Count, ref NoiseGenerator);
            roomGO = Instantiate(roomNormal[oui]); // TODO : add random selection

            DoorsGenerator doorsGenerator = roomGO.transform.Find("Skeleton").transform.Find("Instances_0").GetComponent<DoorsGenerator>();
            doorsGenerator.GenerateSeed(generationParameters);

            int index = 0;
            GameObject entranceDoor = null;
            GameObject exitDoor = null;
            while (entranceDoor == null)
            {
                if (index >= availableDoors.Count)
                {
                    DestroyImmediate(roomGO);
                    DestroyImmediate(exitDoor);
                    //i--; // generation failed then continue
                    return;
                }

                foreach (var door in doorsGenerator.doors)
                {
                    if (door.transform.rotation.eulerAngles.y == (availableDoors[index].transform.rotation.eulerAngles.y + 180f) % 360f)
                    {
                        entranceDoor = door;
                        exitDoor = availableDoors[index];
                        break;
                    }
                }
                index++;
            }

            // sortie.pos = entree.pos + (-entree.arrow.pos + sortie.arrow.pos) + forward * 0.1 (pour avoir un offset
            roomGO.transform.position = entranceDoor.transform.parent.parent.parent.position - entranceDoor.transform.position + exitDoor.transform.position + (-exitDoor.transform.forward * 0.01f);
            Physics.SyncTransforms();

            // ignoble dégueu pas merci dorian (en vrai un peu si quand même)
            // bon sinon j'évite la collide de la salle et la salle exit (forcément que les deux collides putaig)
            BoxCollider roomCollider = roomGO.transform.Find("Skeleton").GetComponentInChildren<BoxCollider>();
            BoxCollider roomColliderExit = exitDoor.transform.parent.parent.GetComponentInChildren<BoxCollider>();
            var truc = roomCollider.BoxOverlap(LayerMask.GetMask("Map"), QueryTriggerInteraction.Collide).Where(tructruc => (tructruc != roomCollider && tructruc != roomColliderExit)).ToArray();
            if (truc.Length != 0)
            {
                availableDoors.Remove(exitDoor);
                DestroyImmediate(roomGO);
                DestroyImmediate(exitDoor);
                //i--; // generation failed then continue
                return;
            }

            doorsGenerator.CloseDoor(entranceDoor);

            DestroyImmediate(exitDoor);
            availableDoors.Remove(exitDoor);

            availableDoors.AddRange(doorsGenerator.doors);
            generationParameters.nbNormal -= doorsGenerator.doors.Count;
        }
        else // first room
        {
            roomGO = Instantiate(roomNormal[0]);

            DoorsGenerator doorsGenerator = roomGO.transform.Find("Skeleton").transform.Find("Instances_0").GetComponent<DoorsGenerator>();
            doorsGenerator.GenerateSeed(generationParameters);
            availableDoors.AddRange(doorsGenerator.doors);
            generationParameters.nbNormal -= doorsGenerator.doors.Count;

            Debug.Log("FIRST ROOM INSTANCE", roomGO);
        }

        roomGO.GetComponentInChildren<RoomGenerator>().GenerateRoomSeed();
        RoomGenerator.RoomGenerated++;
    }

    void GenerateMap(GenerationParameters generationParameters)
    {
        List<GameObject> availableDoors = new List<GameObject>();
        int nbRoom = generationParameters.NbRoom + 1;
        for (int i = 0; i < nbRoom; i++)
        {
            GameObject roomGO;
            if (availableDoors.Count != 0)
            {
                // instantiate room with first availableDoors transform then remove it
                roomGO = Instantiate(roomNormal[0]); // TODO : add random selection

                DoorsGenerator doorsGenerator = roomGO.transform.Find("Skeleton").transform.Find("Instances_0").GetComponent<DoorsGenerator>();
                doorsGenerator.GenerateSeed(generationParameters);

                int index = 0;
                GameObject entranceDoor = null;
                GameObject exitDoor = null;
                while (entranceDoor == null)
                {
                    if (index >= availableDoors.Count)
                    {
                        Debug.LogError("NON");
                        return;
                    }

                    foreach (var door in doorsGenerator.doors)
                    {
                        if (door.transform.rotation.eulerAngles.y == (availableDoors[index].transform.rotation.eulerAngles.y + 180f) % 360f)
                        {
                            entranceDoor = door;
                            exitDoor = availableDoors[index];
                            break;
                        }
                    }
                    index++;
                }

                // sortie.pos = entree.pos + (-entree.arrow.pos + sortie.arrow.pos) + forward * 0.1 (pour avoir un offset
                roomGO.transform.position = entranceDoor.transform.parent.parent.parent.position - entranceDoor.transform.position + exitDoor.transform.position + (-exitDoor.transform.forward * 0.01f);

                // ignoble dégueu pas merci dorian (en vrai un peu si quand même)
                // bon sinon j'évite la collide de la salle et la salle exit (forcément que les deux collides putaig)
                BoxCollider roomCollider = roomGO.transform.Find("Skeleton").GetComponentInChildren<BoxCollider>();
                BoxCollider roomColliderExit = exitDoor.transform.parent.parent.GetComponentInChildren<BoxCollider>();
                var truc = roomCollider.BoxOverlap(LayerMask.GetMask("Map"), QueryTriggerInteraction.Collide).Where(tructruc => (tructruc != roomCollider && tructruc != roomColliderExit)).ToArray();
                if (truc.Length != 0)
                {
                    availableDoors.Remove(exitDoor);
                    DestroyImmediate(roomGO);
                    DestroyImmediate(exitDoor);
                    i--; // generation failed then continue
                    continue;
                }

                doorsGenerator.CloseDoor(entranceDoor);

                DestroyImmediate(exitDoor);
                availableDoors.Remove(exitDoor);

                availableDoors.AddRange(doorsGenerator.doors);
                generationParameters.nbNormal -= doorsGenerator.doors.Count;
            }
            else // first room
            {
                roomGO = Instantiate(roomNormal[0]);

                DoorsGenerator doorsGenerator = roomGO.transform.Find("Skeleton").transform.Find("Instances_0").GetComponent<DoorsGenerator>();
                doorsGenerator.GenerateSeed(generationParameters);
                availableDoors.AddRange(doorsGenerator.doors);
                generationParameters.nbNormal -= doorsGenerator.doors.Count;

                Debug.Log("FIRST ROOM INSTANCE", roomGO);
            }

            roomGO.GetComponentInChildren<RoomGenerator>().GenerateRoomSeed();
            RoomGenerator.RoomGenerated++;
        }
    }
}
