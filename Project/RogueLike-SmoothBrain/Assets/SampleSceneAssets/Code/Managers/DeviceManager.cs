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
    [SerializeField] InputActionAsset playerInput;
    public InputDevice CurrentDevice { get; private set; } = null;
    InputDevice lastUsedDevice = null;
    static private DeviceManager instance;
    public static event Action OnChangedToGamepad;
    public static event Action OnChangedToKB;
    public bool toggleVibrations = true;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Load()
    {
        _ = Instance;
    }

    static public DeviceManager Instance
    {
        get
        {
            if (instance == null)
            {
                Instantiate(Resources.Load<GameObject>(nameof(DeviceManager)));
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

        if (Gamepad.all.Count > 0)
        {
            CurrentDevice = Gamepad.all[0];
            if (Keyboard.current != null)
            {
                lastUsedDevice = Keyboard.current;
            }
            else
            {
                lastUsedDevice = CurrentDevice;
            }
        }
        else
        {
            CurrentDevice = Keyboard.current;
            lastUsedDevice = Keyboard.current;
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

        lastUsedDevice = CurrentDevice;
        CurrentDevice = device;

        CallChangeEvents();
    }

    void OnInputSystemDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (IsSameDevice(device))
            return;

        if (change == InputDeviceChange.Removed || change == InputDeviceChange.Disconnected || change == InputDeviceChange.Disabled)
        {
            CurrentDevice = lastUsedDevice;
        }
        else
        {
            lastUsedDevice = CurrentDevice;
            CurrentDevice = device;
        }

        CallChangeEvents();
    }

    bool IsSameDevice(InputDevice device)
    {
        return CurrentDevice == device || (device is Keyboard && CurrentDevice is Mouse) || (device is Mouse && CurrentDevice is Keyboard);
    }

    void CallChangeEvents()
    {
        if (instance != null)
        {
            StopAllCoroutines();
        }

        if (CurrentDevice is Gamepad)
        {
            if (debugText != null)
            {
                debugText.SetText("GAMEPAD");
            }
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            playerInput.FindActionMap("Keyboard", throwIfNotFound: true).Disable();
            playerInput.FindActionMap("Gamepad", throwIfNotFound: true).Enable();
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
            playerInput.FindActionMap("Gamepad", throwIfNotFound: true).Disable();
            playerInput.FindActionMap("Keyboard", throwIfNotFound: true).Enable();
            OnChangedToKB?.Invoke();
        }
    }

    public bool IsPlayingKB()
    {
        return CurrentDevice is not Gamepad;
    }

    public bool IsSupportingVibrations()
    {
        return CurrentDevice is Gamepad && (CurrentDevice is XInputController || CurrentDevice is DualShockGamepad);
    }

    public void ApplyVibrations(float lowFrequency, float highFrequency, float duration)
    {
        StopAllCoroutines();
        if (IsSupportingVibrations() && toggleVibrations)
        {
            lowFrequency = Mathf.Clamp(lowFrequency, 0f, 1f);
            highFrequency = Mathf.Clamp(highFrequency, 0f, 1f);

            (CurrentDevice as Gamepad).SetMotorSpeeds(lowFrequency, highFrequency);
            StartCoroutine(StopVibration(CurrentDevice as Gamepad, duration));
        }
    }

    public void ApplyVibrationsInfinite(float lowFrequency, float highFrequency)
    {
        StopAllCoroutines();
        if (IsSupportingVibrations() && toggleVibrations)
        {
            lowFrequency = Mathf.Clamp(lowFrequency, 0f, 1f);
            highFrequency = Mathf.Clamp(highFrequency, 0f, 1f);
            StartCoroutine(VibrationsInfiniteCoroutine(lowFrequency, highFrequency));
        }
    }

    IEnumerator VibrationsInfiniteCoroutine(float lowFrequency, float highFrequency)
    {
        while (true)
        {
            (CurrentDevice as Gamepad).SetMotorSpeeds(lowFrequency, highFrequency);
            yield return new WaitForSeconds(1f);
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
        if (CurrentDevice is Gamepad)
        {
            (CurrentDevice as Gamepad).SetMotorSpeeds(0f, 0f);
        }
    }
}