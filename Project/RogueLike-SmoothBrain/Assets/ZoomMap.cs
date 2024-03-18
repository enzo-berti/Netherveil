using UnityEngine;

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
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + Input.mouseScrollDelta.y * 5, 25, 110);
    }
}
