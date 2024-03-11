using UnityEngine;

public class MiniMapCam : MonoBehaviour
{
    private Transform player;

    void Start()
    {
        if (player == null)
            player = FindObjectOfType<PlayerController>().transform;
    }

    void LateUpdate()
    {
        if (player)
            SetPosition();
    }

    private void SetPosition()
    {
        var newPos = player.position;
        newPos.y = transform.position.y;
        transform.position = newPos;
    }
}
