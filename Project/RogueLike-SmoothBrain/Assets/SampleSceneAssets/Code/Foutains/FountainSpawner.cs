using UnityEngine;

public class FountainSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] fountains;

    private void Start()
    {
        Instantiate(fountains[Random.Range(0, fountains.Length)], transform);
    }
}
