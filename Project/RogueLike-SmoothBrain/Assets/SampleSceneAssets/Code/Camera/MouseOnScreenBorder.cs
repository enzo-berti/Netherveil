using Netherveil.Inputs;
using UnityEngine;
using static Entity;

public class MouseOnScreenBorder : MonoBehaviour
{
    private Vector3 targetPosition;
    private Vector3 currentVelocity = Vector3.zero;
    private float smoothTime = 0.2f;
    private Transform playerTransform;
    private PlayerInput playerInput;
    private PlayerInputMap inputs;
    private Hero player;

    private void OnEnable()
    {
        inputs.Enable();
    }


    private void OnDisable()
    {
        inputs.Disable();
    }

    private void Awake()
    {
        inputs = new PlayerInputMap();
    }

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        playerInput = playerTransform.gameObject.GetComponent<PlayerInput>();
    }

    void FixedUpdate()
    {
        if (DeviceManager.Instance.IsPlayingKB())
        {
            CollidMouseScreen();
        }
        else
        {
            CollideJoystickScreen();
        }

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

    private void CollideJoystickScreen()
    {
        Vector2 joyStickInput = inputs.Gamepad.CamLookAway.ReadValue<Vector2>();
        Vector3 offsetX = Vector3.zero;
        Vector3 offsetY = Vector3.zero;
        float offsetDistCam = 2f;

        if (joyStickInput.x > 0.5f)
        {
            offsetX = Camera.main.transform.right * offsetDistCam;
        }
        else if (joyStickInput.x < -0.5f)
        {
            offsetX = -Camera.main.transform.right * offsetDistCam;
        }

        if (joyStickInput.y > 0.5f)
        {
            offsetY = Camera.main.transform.up * offsetDistCam;
        }
        else if (joyStickInput.y < -0.5f)
        {
            offsetY = -Camera.main.transform.up * offsetDistCam;
        }

        targetPosition = playerTransform.position + offsetX + offsetY;
    }

    private void ChangeOffsetPos()
    {
        if (targetPosition != playerTransform.position && playerInput.Direction == Vector2.zero && player.State != (int)EntityState.DEAD)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, playerTransform.position, ref currentVelocity, smoothTime / 3);
        }
    }
}