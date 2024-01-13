using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class MeshButton : MonoBehaviour
{
    private bool isHovered = false;

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
                    OnHoverEnter();
                }

                isHovered = true;

                if (Input.GetMouseButtonDown(0))
                {
                    OnPress();
                }

                if (Input.GetMouseButtonUp(0))
                {
                    OnRelease();
                }
            }
            else
            {
                if (isHovered)
                {
                    OnHoverExit();
                }

                isHovered = false;
            }
        }
        else
        {
            if (isHovered)
            {
                OnHoverExit();
            }

            isHovered = false;
        }
    }

    void OnHoverEnter()
    {
        onHoverEnter?.Invoke();
    }

    void OnHoverExit()
    {
        onHoverExit?.Invoke();
    }

    void OnPress()
    {
        onPress?.Invoke();
    }

    void OnRelease()
    {
        onRelease?.Invoke();
    }
}
