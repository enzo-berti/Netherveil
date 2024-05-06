using UnityEngine;
using UnityEngine.InputSystem;

public class BigMapCam : MonoBehaviour
{
    //zoom
    public static Vector2 mouseScrollDelta;
    Camera cam;

    //move
    [SerializeField] private RectTransform mapRect;
    float currentZoom;
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

        currentZoom = Mathf.Clamp(cam.orthographicSize +
            (DeviceManager.Instance.IsPlayingKB() ? -Input.mouseScrollDelta.y * 5 : gamepad.leftTrigger.EvaluateMagnitude() - gamepad.rightTrigger.EvaluateMagnitude()), 25, 110);

        cam.orthographicSize = currentZoom;
    }

    private void MoveCam()
    {
        if (DeviceManager.Instance.IsPlayingKB())
            CollideMouseScreen();
        else
            CollideJoystickScreen();
    }

    private Vector3 GetCameraOriented(Vector3 vector)
    {
        Vector3 camRight, camForward;
        camForward = transform.forward;
        camRight = transform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward = camForward.normalized;
        camRight = camRight.normalized;
        
        return camForward * vector.y + camRight * vector.x;
    }

    private void CollideMouseScreen()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(mapRect, Input.mousePosition, null, out Vector2 localPoint);
        localPoint /= mapRect.sizeDelta / 2f;

        localPoint.x = Mathf.Clamp(localPoint.x, -1f, 1f);
        localPoint.y = Mathf.Clamp(localPoint.y, -1f, 1f);

        Vector3 localPointToCamera = GetCameraOriented(localPoint);

        transform.position = Vector3.Lerp(transform.position, playerTransform.position + localPointToCamera * Mathf.Abs(currentZoom - 100f) * 10f, Time.deltaTime * 20f);
    }

    private void CollideJoystickScreen()
    {
        Gamepad gamepad = DeviceManager.Instance.CurrentDevice as Gamepad;
        Vector2 joyStickInput = gamepad.rightStick.value;

        Vector3 localPointToCamera = GetCameraOriented(joyStickInput);

        transform.position = Vector3.Lerp(transform.position, playerTransform.position + localPointToCamera * 50f, Time.deltaTime * 20f);
    }
}
