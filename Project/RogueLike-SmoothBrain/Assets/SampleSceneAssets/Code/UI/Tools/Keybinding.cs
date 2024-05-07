using UnityEngine;
using UnityEngine.InputSystem;

public class Keybinding : MonoBehaviour
{
    [SerializeField] InputActionAsset playerInput;

    public void ResetCurrentBindings()
    {
        if(DeviceManager.Instance.IsPlayingKB())
        {
            playerInput.FindActionMap("Keyboard", throwIfNotFound: true).RemoveAllBindingOverrides();
        }
        else
        {
            playerInput.FindActionMap("Gamepad", throwIfNotFound: true).RemoveAllBindingOverrides();
        }
    }

    public void ResetKeyboardBindings()
    {
        playerInput.FindActionMap("Keyboard", throwIfNotFound: true).RemoveAllBindingOverrides();
    }

    public void ResetGamepadBindings()
    {
        playerInput.FindActionMap("Gamepad", throwIfNotFound: true).RemoveAllBindingOverrides();
    }
}
