using System.Collections.Generic;
using UnityEngine;

struct GenerationParameters
{
    public int nbNormal;
    public int nbTreasure;
    public int nbChallenge;
    public int nbMerchant;
    public int nbSecret;
    public int nbMiniBoss;
    public int nbRoomMiniBoss;
    public int nbRoomBoss;

    public readonly int nbRoom
    {
        get { return nbNormal + nbTreasure + nbChallenge + nbMerchant + nbSecret + nbMiniBoss + nbRoomMiniBoss + nbRoomBoss; }
    }
}

public class MapGenerator : MonoBehaviour
{
    public static int RoomGenerated { get; private set; } = 0;

    [SerializeField] private List<GameObject> roomNormal = new List<GameObject>();
    [SerializeField] private List<GameObject> roomTreasure = new List<GameObject>();
    [SerializeField] private List<GameObject> roomChallenge = new List<GameObject>();
    [SerializeField] private List<GameObject> roomMerchant = new List<GameObject>();
    [SerializeField] private List<GameObject> roomSecret = new List<GameObject>();
    [SerializeField] private List<GameObject> roomMiniBoss = new List<GameObject>();
    [SerializeField] private List<GameObject> roomBoss = new List<GameObject>();

    private void Start()
    {
        GenerationParameters generationParam = new GenerationParameters();
        generationParam.nbNormal = 10;

        GenerateMap(generationParam);
    }

    void GenerateMap(GenerationParameters generationParameters)
    {
        for (int i = 0; i < generationParameters.nbRoom; i++)
        {
            GenerateRoom(4);
            RoomGenerated++;
        }
    }

    void GenerateRoom(int numberOfDoor = 1)
    {
        var go = Instantiate(roomNormal[0]);
        go.GetComponentInChildren<RoomGenerator>().Generate(); // generate room

        var doorsGO = go.transform.Find("NetherVeilProceduralRooms_merge2_merge2_1_bakedClone");
        int numberOfDoors = doorsGO.childCount;
    }
}
