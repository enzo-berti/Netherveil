using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class InputDeviceManager : MonoBehaviour
{
    //encore à modifier pour trigger les events aussi quand on déconnecte/connecte un device?
    InputDevice lastUsedDevice = null;
    public static event Action OnChangedToController;
    public static event Action OnChangedToKB;
    void Start()
    {
        // Subscribe to control scheme change event
        InputSystem.onEvent += OnInputSystemEvent;

        // Initially check the current control scheme
        //UpdateControlScheme();
        InputSystem.onDeviceChange += OnInputSystemDeviceChange;
    }

    void OnInputSystemEvent(InputEventPtr eventPtr, InputDevice device)
    {
        if (lastUsedDevice == device)
            return;

        // Some devices like to spam events like crazy.
        // Example: PS4 controller on PC keeps triggering events without meaningful change.
        var eventType = eventPtr.type;
        if (eventType == StateEvent.Type)
        {
            // Go through the changed controls in the event and look for ones actuated
            // above a magnitude of a little above zero.
            if (!eventPtr.EnumerateChangedControls(device: device, magnitudeThreshold: 0.0001f).Any())
                return;
        }


        lastUsedDevice = device;
        if(lastUsedDevice is Gamepad)
        {
            //Debug.Log(lastUsedDevice);
            OnChangedToController?.Invoke();
        }
        else
        {
            Debug.Log("TEST");
            OnChangedToKB?.Invoke();
        }
    }

    void OnInputSystemDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (lastUsedDevice == device)
            return;

        lastUsedDevice = device;

        if (lastUsedDevice is Gamepad)
        {
            //Debug.Log(lastUsedDevice);
            OnChangedToController?.Invoke();
        }
        else
        {
            Debug.Log("TEST");
            OnChangedToKB?.Invoke();
        }
    }
}