using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace MeshUI
{
    [RequireComponent(typeof(Collider))]
    public class MeshButton : MonoBehaviour
    {
        [SerializeField] private MeshNavigation navigation;
        public MeshNavigation Navigation => navigation;

        private bool isHovered = false;
        private bool isPressed = false;

        [Header("Events"), Space]
        [SerializeField] private UnityEvent onHoverEnter;
        [SerializeField] private UnityEvent onHoverExit;
        [SerializeField] private UnityEvent onPress;
        [SerializeField] private UnityEvent onRelease;

        private void Update()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    if (!isHovered)
                    {
                        OnHover();
                    }

                    isHovered = true;

                    if (Input.GetMouseButtonDown(0))
                    {
                        if (EventSystem.current.IsPointerOverGameObject())
                            return;

                        OnPress();
                        isPressed = true;
                    }
                }
                else
                {
                    if (isHovered)
                    {
                        OnOut();
                    }

                    isHovered = false;
                }
            }
            else
            {
                if (isHovered)
                {
                    OnOut();
                }

                isHovered = false;
            }

            if (Input.GetMouseButtonUp(0) && isPressed)
            {
                OnRelease();
                isPressed = false;
            }
        }

        public void OnHover()
        {
            onHoverEnter?.Invoke();
        }

        public void OnOut()
        {
            onHoverExit?.Invoke();
        }

        public void OnPress()
        {
            onPress?.Invoke();
        }

        public void OnRelease()
        {
            onRelease?.Invoke();
        }
    }
}
