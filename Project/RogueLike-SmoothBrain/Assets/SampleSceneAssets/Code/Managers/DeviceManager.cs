using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.XInput;

public class DeviceManager : MonoBehaviour
{
    //à tout moment si tu bouges la manette en meme temps qu'une touche de clavier ou la souris c'est le bordel mais t'as qu'à pas être un fdp aussi
    [SerializeField] TMP_Text debugText;
    public UnityEngine.InputSystem.PlayerInput playerInput;
    InputDevice currentDevice = null;
    InputDevice lastUsedDevice = null;
    static private DeviceManager instance;
    public static event Action OnChangedToGamepad;
    public static event Action OnChangedToKB;
    public bool toggleVibrations = true;

    static public DeviceManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject(nameof(DeviceManager));
                obj.AddComponent<DeviceManager>();
            }

            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnDestroy()
    {
        ForceStopVibrations();
    }

    void Start()
    {
        InputSystem.onEvent += OnInputSystemEvent;
        InputSystem.onDeviceChange += OnInputSystemDeviceChange;
        if(Gamepad.all.Count > 0)
        {
            currentDevice = Gamepad.all[0];
            if(Keyboard.current != null)
            {
                lastUsedDevice = Keyboard.current;
            }
            else
            {
                lastUsedDevice = currentDevice;
            }
        }
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
            if (playerInput != null)
            {
                playerInput.currentActionMap.Disable();
                playerInput.actions.FindActionMap("Gamepad", true).Enable();
                playerInput.SwitchCurrentActionMap("Gamepad");
            }
            OnChangedToGamepad?.Invoke();
        }
        else
        {
            if (debugText != null)
            {
                debugText.SetText("KB");
            }
            //should be confined here but for debug reasons we'll put None
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if(playerInput != null)
            {
                playerInput.currentActionMap.Disable();
                playerInput.actions.FindActionMap("Keyboard", true).Enable();
                playerInput.SwitchCurrentActionMap("Keyboard");
            }
            OnChangedToKB?.Invoke();
        }
    }

    public bool IsPlayingKB()
    {
        return currentDevice is not Gamepad;
    }

    public bool IsSupportingVibrations()
    {
        return currentDevice is Gamepad && (currentDevice is XInputController || currentDevice is DualShockGamepad);
    }

    public void ApplyVibrations(float lowFrequency, float highFrequency, float duration)
    {
        if(IsSupportingVibrations() && toggleVibrations)
        {
            lowFrequency = Mathf.Clamp(lowFrequency, 0f, 1f);
            highFrequency = Mathf.Clamp(highFrequency, 0f, 1f);

            (currentDevice as Gamepad).SetMotorSpeeds(lowFrequency, highFrequency);
            StartCoroutine(StopVibration(currentDevice as Gamepad, duration));
        }
    }

    private IEnumerator StopVibration(Gamepad gamepad, float duration)
    {
        yield return new WaitForSeconds(duration);

        // Stop the vibration after the specified duration
        gamepad.SetMotorSpeeds(0f, 0f);
    }

    public void ForceStopVibrations()
    {
        StopAllCoroutines();
        if (currentDevice is Gamepad)
        {
            (currentDevice as Gamepad).SetMotorSpeeds(0f, 0f);
        }
    }
}