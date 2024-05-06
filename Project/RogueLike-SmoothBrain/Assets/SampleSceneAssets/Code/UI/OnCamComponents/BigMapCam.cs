using UnityEngine;
using UnityEngine.InputSystem;

public class BigMapCam : MonoBehaviour
{
    //zoom
    public static Vector2 mouseScrollDelta;
    Camera cam;

    //move
    [SerializeField] private RectTransform mapRect;
    private Vector3 targetPosition;
    private Vector3 currentVelocity = Vector3.zero;
    private float smoothTime = 0.2f;
    private Transform playerTransform;

    void Start()
    {
        cam = GetComponent<Camera>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
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
            CollideMouseScreen();
        else
            CollideJoystickScreen();
    }

    private void CollideMouseScreen()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(mapRect, Input.mousePosition, null, out Vector2 localPoint);
        localPoint /= mapRect.sizeDelta / 2.0f;

        localPoint.x = Mathf.Clamp(localPoint.x, -1.0f, 1.0f);
        localPoint.y = Mathf.Clamp(localPoint.y, -1.0f, 1.0f);

        transform.position = Vector3.Lerp(transform.position, playerTransform.position + new Vector3(localPoint.x * 100.0f, 0.0f, localPoint.y * 50.0f), Time.deltaTime * 20.0f);
    }

    private void CollideJoystickScreen()
    {
        Gamepad gamepad = DeviceManager.Instance.CurrentDevice as Gamepad;

        Vector2 joyStickInput = gamepad.leftStick.value;
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
}
