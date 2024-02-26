using UnityEngine;

public class FixedMouseMouvementCamera : MonoBehaviour
{
    private Vector3 StartCameraRotation;
    [SerializeField] private Vector2 offsetMovement = new Vector2(10f, 10f);

    private void Start()
    {
        StartCameraRotation = transform.rotation.eulerAngles;
    }

    private void Update()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        Vector2 mouseViewportPositionNormalize = (Camera.main.ScreenToViewportPoint(mouseScreenPosition) - Vector3.one / 2f) * 2f;
        Vector2 mouseViewportPositionClamp = new Vector2(Mathf.Clamp(mouseViewportPositionNormalize.x, -1, 1), Mathf.Clamp(mouseViewportPositionNormalize.y, -1, 1));
        Vector2 mouseViewportPosition = mouseViewportPositionClamp * offsetMovement;
        Vector3 angleMovement = new Vector3(-mouseViewportPosition.y, mouseViewportPosition.x, 0f);
        transform.eulerAngles = StartCameraRotation + angleMovement;
    }
}
