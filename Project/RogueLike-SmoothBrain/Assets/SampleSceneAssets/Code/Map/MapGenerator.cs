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
    private static int roomGenerated = 0;

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

        Generate(generationParam);
    }

    void Generate(GenerationParameters generationParameters)
    {
        
    }
}
