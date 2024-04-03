using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace MeshUI
{
    public class EventSystemMesh : MonoBehaviour
    {
        public static EventSystemMesh current;
        public MeshButton meshButtonFocus;
        private InputSystemUIInputModule module;

        private void Awake()
        {
            current = this;
        }

        private void Start()
        {
            meshButtonFocus.OnHover();
        }

        private void OnEnable()
        {
            module = GetComponent<InputSystemUIInputModule>();

            module.move.action.performed += Navigate;
            module.submit.action.started += Press;
        }

        private void OnDisable()
        {
            module.move.action.performed -= Navigate;
            module.submit.action.started -= Press;
        }

        private void Press(InputAction.CallbackContext context)
        {
            meshButtonFocus.OnPress();
        }

        private void Navigate(InputAction.CallbackContext context)
        {
            if (!enabled)
                return;

            Vector2 direction = context.ReadValue<Vector2>();

            if (direction == Vector2.zero)
                return;

            MeshButton meshButton = meshButtonFocus.Navigation.Navigate(direction);

            if (meshButton != null)
            {
                meshButtonFocus.OnOut();
                meshButtonFocus = meshButton;
                meshButtonFocus.OnHover();
            }
        }
    }
}
