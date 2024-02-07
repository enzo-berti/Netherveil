using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class InputDeviceManager : MonoBehaviour
{
    //à tout moment si tu bouges la manette en meme temps qu'une touche de clavier ou la souris c'est le bordel mais t'as qu'à pas être un fdp aussi
    [SerializeField] TMP_Text debugText;
    InputDevice currentDevice = null;
    InputDevice lastUsedDevice = null;
    public static event Action OnChangedToGamepad;
    public static event Action OnChangedToKB;

    static private InputDeviceManager instance;
    static public InputDeviceManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject(typeof(InputDeviceManager).Name);
                instance = obj.AddComponent<InputDeviceManager>();
                DontDestroyOnLoad(obj);
            }

            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
    }


    void Start()
    {
        InputSystem.onEvent += OnInputSystemEvent;
        InputSystem.onDeviceChange += OnInputSystemDeviceChange;
    }

    void OnInputSystemEvent(InputEventPtr eventPtr, InputDevice device)
    {
        if (IsSameDevice(device))
        {
            return;
        }

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

        lastUsedDevice = currentDevice;
        currentDevice = device;

        CallChangeEvents();
    }

    void OnInputSystemDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (change == InputDeviceChange.Removed || change == InputDeviceChange.Disconnected || change == InputDeviceChange.Disabled)
        {
            currentDevice = lastUsedDevice;
        }
        else if (IsSameDevice(device))
        {
            return;
        }
        else
        {
           lastUsedDevice = currentDevice;
            currentDevice = device;
        }

        CallChangeEvents();
    }

    bool IsSameDevice(InputDevice device)
    {
        return currentDevice == device || (device is Keyboard && currentDevice is Mouse) || (device is Mouse && currentDevice is Keyboard);
    }

    void CallChangeEvents()
    {
        if (currentDevice is Gamepad)
        {
            if(debugText != null)
            {
                debugText.SetText("GAMEPAD");
            }
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            OnChangedToGamepad?.Invoke();
        }
        else
        {
            if (debugText != null)
            {
                debugText.SetText("KB");
            }
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            OnChangedToKB?.Invoke();
        }
    }
}