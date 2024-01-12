using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(prefab, new Vector3(10, 0, 0), Quaternion.identity);
        Instantiate(prefab, new Vector3(-10, 0, 0), Quaternion.identity);
        Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
