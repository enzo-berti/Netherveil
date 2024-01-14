using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class MeshButton : MonoBehaviour
{
    private bool isHovered = false;
    private bool isPressed = false;

    [Header("Events"), Space]
    [SerializeField] private UnityEvent onHoverEnter;
    [SerializeField] private UnityEvent onHoverExit;
    [SerializeField] private UnityEvent onPress;
    [SerializeField] private UnityEvent onRelease;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == gameObject)
            {
                if (!isHovered)
                {
                    onHoverEnter?.Invoke();
                }

                isHovered = true;

                if (Input.GetMouseButtonDown(0))
                {
                    onPress?.Invoke();
                    isPressed = true;
                }
            }
            else
            {
                if (isHovered)
                {
                    onHoverExit?.Invoke();
                }

                isHovered = false;
            }
        }
        else
        {
            if (isHovered)
            {
                onHoverExit?.Invoke();
            }

            isHovered = false;
        }

        if (Input.GetMouseButtonUp(0) && isPressed)
        {
            onRelease?.Invoke();
            isPressed = false;
        }
    }
}
