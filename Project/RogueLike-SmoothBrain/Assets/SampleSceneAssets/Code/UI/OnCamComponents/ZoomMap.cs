using UnityEngine;
using UnityEngine.InputSystem;

public class ZoomMap : MonoBehaviour
{
    public static Vector2 mouseScrollDelta;
    Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        Gamepad gamepad = DeviceManager.Instance.CurrentDevice as Gamepad;

        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + 
            (DeviceManager.Instance.IsPlayingKB() ? -Input.mouseScrollDelta.y * 5 : gamepad.leftTrigger.EvaluateMagnitude() - gamepad.rightTrigger.EvaluateMagnitude()), 25, 110);
    }
}
