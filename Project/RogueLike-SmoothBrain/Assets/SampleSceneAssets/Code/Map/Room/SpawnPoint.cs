using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private void Start()
    {
        Utilities.Player.transform.position = transform.position;
    }
}