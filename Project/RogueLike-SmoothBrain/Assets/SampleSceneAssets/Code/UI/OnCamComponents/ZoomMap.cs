using Netherveil.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static Entity;

public class ZoomMap : MonoBehaviour
{
    //zoom
    public static Vector2 mouseScrollDelta;
    Camera cam;

    //move
    [SerializeField] GameObject map;
    private Image mapImage;
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
        cam = GetComponent<Camera>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        playerInput = playerTransform.gameObject.GetComponent<PlayerInput>();
        mapImage = map.GetComponent<Image>();
    }

    private void Update()
    {
        ZoomCam();
        MoveCam();
    }

    private void ZoomCam()
    {
        Gamepad gamepad = DeviceManager.Instance.CurrentDevice as Gamepad;

        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize +
            (DeviceManager.Instance.IsPlayingKB() ? -Input.mouseScrollDelta.y * 5 : gamepad.leftTrigger.EvaluateMagnitude() - gamepad.rightTrigger.EvaluateMagnitude()), 25, 110);
    }

    private void MoveCam()
    {
        if (DeviceManager.Instance.IsPlayingKB())
        {
            //CollidMouseScreen(); ça bug de bz
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
        float offsetDistCam = 50f;

      /*  if (mousepos.x > mapImage.transform.position.x + mapImage.rectTransform.rect.width/2 - offsetDistBorder)
        {
            offsetX = Camera.main.transform.right * offsetDistCam;
        }
        else if (mousepos.x < mapImage.transform.position.x + offsetDistBorder)
        {
            offsetX = -Camera.main.transform.right * offsetDistCam;
        }*/

        if (mousepos.y > mapImage.rectTransform.rect.position.y + mapImage.rectTransform.rect.height/2 - offsetDistBorder)
        {
            offsetY = Camera.main.transform.up * offsetDistCam;
        }
        else if (mousepos.y < mapImage.rectTransform.rect.position.y - mapImage.rectTransform.rect.height/2 + offsetDistBorder)
        {
            offsetY = -Camera.main.transform.up * offsetDistCam;
        }

        float truc = mapImage.rectTransform.rect.position.y + mapImage.rectTransform.rect.height / 2 - offsetDistBorder;
        Debug.Log("mousePosY : " + mousepos.y);
        Debug.Log("position.y : " + truc);

        targetPosition = playerTransform.position + offsetX + offsetY;
    }

    private void CollideJoystickScreen()
    {
        Vector2 joyStickInput = inputs.Gamepad.CamLookAway.ReadValue<Vector2>();
        Vector3 offsetX = Vector3.zero;
        Vector3 offsetY = Vector3.zero;
        float offsetDistCam = 200f;

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
        /*else
        {
            if(bouton recentrer appuyé)
            {
                transform.position = Vector3.SmoothDamp(transform.position, playerTransform.position, ref currentVelocity, smoothTime / 3);
            }
        }*/
    }
}
