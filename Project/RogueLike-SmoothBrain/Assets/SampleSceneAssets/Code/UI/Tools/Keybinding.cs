using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Keybinding : MonoBehaviour
{
    [SerializeField] List<GameObject> KBBindings;
    [SerializeField] List<GameObject> GamepadBindings;
    [SerializeField] InputActionAsset playerInput;

    void Start()
    {
        DeviceManager.OnChangedToKB += SwitchBindings;
        DeviceManager.OnChangedToGamepad += SwitchBindings;
    }

    private void OnDestroy()
    {
        DeviceManager.OnChangedToKB -= SwitchBindings;
        DeviceManager.OnChangedToGamepad -= SwitchBindings;
    }

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

    private void SwitchBindings()
    {
        if (this != null)
        {
            bool isPlayingKB = DeviceManager.Instance.IsPlayingKB();

            foreach (var binding in KBBindings)
            {
                binding.SetActive(isPlayingKB);
            }
            foreach (var binding in GamepadBindings)
            {
                binding.SetActive(!isPlayingKB);
            }
        }
    }
}
