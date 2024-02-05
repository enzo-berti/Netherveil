using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCam : MonoBehaviour
{
    [SerializeField] Transform player;

    void Start()
    {
        SetPosition();
    }

    void LateUpdate()
    {
        if (player != null)
        {
            SetPosition();
        }
    }

    private void SetPosition()
    {
        var newPos = player.position;
        newPos.y = transform.position.y;
        transform.position = newPos;
    }
}
