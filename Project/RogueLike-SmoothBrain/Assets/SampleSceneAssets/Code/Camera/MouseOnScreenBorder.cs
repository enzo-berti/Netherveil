using UnityEngine;

public class MouseOnScreenBorder : MonoBehaviour
{
    private Vector3 targetPosition;
    private Vector3 currentVelocity = Vector3.zero;
    private float smoothTime = 0.5f; 
    private Transform playerTransform;
    private PlayerController playerController;


    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = playerTransform.gameObject.GetComponent<PlayerController>();
    }

    void FixedUpdate()
    {
        CollidMouseScreen();
        ChangeOffsetPos();
    }

    private void CollidMouseScreen()
    {
        Vector2 mousepos = Input.mousePosition;
        Vector3 offsetX = Vector3.zero;
        Vector3 offsetY = Vector3.zero;
        float offsetDistBorder = 10f; 
        float offsetDistCam = 2f;

        if (mousepos.x > Screen.width - offsetDistBorder)
        {
            offsetX = Camera.main.transform.right * offsetDistCam;
        }
        else if (mousepos.x < offsetDistBorder)
        {
            offsetX = -Camera.main.transform.right * offsetDistCam;
        }

        if (mousepos.y > Screen.height - offsetDistBorder)
        {
            offsetY = Camera.main.transform.up * offsetDistCam;
        }
        else if (mousepos.y < offsetDistBorder)
        {
            offsetY = -Camera.main.transform.up * offsetDistCam;
        }

        targetPosition = playerTransform.position + offsetX + offsetY;
    }

    private void ChangeOffsetPos()
    {
        if (targetPosition != playerTransform.position && playerController.Direction == Vector2.zero)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);
        }
        else
        {
            transform.position = playerTransform.position;
        }
    }
}