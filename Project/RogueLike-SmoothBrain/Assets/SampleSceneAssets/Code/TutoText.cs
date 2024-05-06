using Map.Generation;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;

public class TutoText : MonoBehaviour
{
    [SerializeField] List<InputActionReference> keyboardActions;
    [SerializeField] List<InputActionReference> gamepadActions;
    [SerializeField] InputBinding.DisplayStringOptions displayStringOptions;

    string initText = string.Empty;
    TMP_Text text;

    void Start()
    {
        // Temporaire
        MapGenerator mapGen = FindAnyObjectByType<MapGenerator>();
        if (mapGen.stage > 1)
        {
            Destroy(gameObject);
        }
        //

        text = GetComponent<TMP_Text>();
        initText = text.text;

        PauseMenu.OnUnpause += UpdateBindingDisplayString;

        UpdateBindingDisplayString();
    }

    private void OnEnable()
    {
        if (Application.isPlaying)
        {
            DeviceManager.OnChangedToKB += UpdateBindingDisplayString;
            DeviceManager.OnChangedToGamepad += UpdateBindingDisplayString;
        }
    }

    private void OnDisable()
    {
        DeviceManager.OnChangedToKB -= UpdateBindingDisplayString;
        DeviceManager.OnChangedToGamepad -= UpdateBindingDisplayString;
        PauseMenu.OnUnpause -= UpdateBindingDisplayString;
    }

    private void OnDestroy()
    {
        DeviceManager.OnChangedToKB -= UpdateBindingDisplayString;
        DeviceManager.OnChangedToGamepad -= UpdateBindingDisplayString;
        PauseMenu.OnUnpause -= UpdateBindingDisplayString;
    }

    private void UpdateBindingDisplayString()
    {
        List<InputActionReference> actionRefs = GetCurrentAction();

        int nbDifferentTexts = 0;
        for (int i = 0; i < actionRefs.Count; i++)
        {
            if (actionRefs[i].action.GetBindingDisplayString(displayStringOptions) != text.text)
                nbDifferentTexts++;
        }

        if (nbDifferentTexts > 0)
        {
            string textString = initText;

            if (actionRefs[0].action.name == "Movement" && DeviceManager.Instance.IsPlayingKB())
            {
                textString = textString.Replace("^", "<sprite name=\"" + GetDisplayString(actionRefs[0], 1).GetCamelCase() + "\">");
                if (textString.Contains("$"))
                {
                    textString = textString.Replace("$", "<sprite name=\"" + GetDisplayString(actionRefs[0], 3).GetCamelCase() + "\">");
                }
                if (textString.Contains("%"))
                {
                    textString = textString.Replace("%", "<sprite name=\"" + GetDisplayString(actionRefs[0], 2).GetCamelCase() + "\">");
                }
                if (textString.Contains("*"))
                {
                    textString = textString.Replace("*", "<sprite name=\"" + GetDisplayString(actionRefs[0], 4).GetCamelCase() + "\">");
                }
            }
            else if (actionRefs[0].action.name == "Movement" && !DeviceManager.Instance.IsPlayingKB())
            {
                textString = textString.Replace("^", "<sprite name=\"" + "leftStickPress" + (DeviceManager.Instance.CurrentDevice is DualShockGamepad ? "_ps" : "_xbox")  + "\">");
                textString = textString.Replace("$", string.Empty);
                textString = textString.Replace("%", string.Empty);
                textString = textString.Replace("*", string.Empty);
            }
            else
            {
                textString = textString.Replace("^", "<sprite name=\"" + GetDisplayString(actionRefs[0]).GetCamelCase() + "\">");
                if (textString.Contains("$"))
                {
                    textString = textString.Replace("$", "<sprite name=\"" + GetDisplayString(actionRefs[1]).GetCamelCase() + "\">");
                }
            }
            text.text = textString;
        }
    }

    private List<InputActionReference> GetCurrentAction()
    {
        if (DeviceManager.Instance.IsPlayingKB())
        {
            return keyboardActions;
        }
        else
        {
            return gamepadActions;
        }
    }

    private string GetDisplayString(InputActionReference actionRef, int bindingIndex = 0)
    {
        // Get display string from action.
        var action = actionRef != null ? actionRef.action : null;
        string displayString = string.Empty;

        if (action != null)
        {

            displayString = GetAppropriateKeyString(actionRef, bindingIndex);
            if (!DeviceManager.Instance.IsPlayingKB() && DeviceManager.Instance.CurrentDevice is DualShockGamepad)
            {
                displayString += "_ps";
            }
            else if (!DeviceManager.Instance.IsPlayingKB())
            {
                displayString += "_xbox";
            }
        }

        return displayString;
    }

    private string GetAppropriateKeyString(InputActionReference actionRef, int bindingIndex = 0)
    {
        var action = actionRef != null ? actionRef.action : null;
        string bindingDisplayString = action.GetBindingDisplayString(bindingIndex, out _, out string controlPath, displayStringOptions);

        switch (controlPath)
        {
            //keyboard bindings
            case "escape": return controlPath;
            case "space": return controlPath;
            case "enter": return controlPath;
            case "tab": return controlPath;
            case "backquote": return controlPath;
            case "quote": return controlPath;
            case "semicolon": return controlPath;
            case "comma": return controlPath;
            case "period": return controlPath;
            case "slash": return controlPath;
            case "backslash": return controlPath;
            case "leftBracket": return controlPath;
            case "rightBracket": return controlPath;
            case "minus": return controlPath;
            case "equals": return controlPath;
            case "upArrow": return controlPath;
            case "downArrow": return controlPath;
            case "leftArrow": return controlPath;
            case "rightArrow": return controlPath;
            case "1": return controlPath;
            case "2": return controlPath;
            case "3": return controlPath;
            case "4": return controlPath;
            case "5": return controlPath;
            case "6": return controlPath;
            case "7": return controlPath;
            case "8": return controlPath;
            case "9": return controlPath;
            case "0": return controlPath;
            case "leftShift": return controlPath;
            case "rightShift": return controlPath;
            case "shift": return controlPath;
            case "leftAlt": return controlPath;
            case "rightAlt": return controlPath;
            case "alt": return controlPath;
            case "leftCtrl": return controlPath;
            case "rightCtrl": return controlPath;
            case "ctrl": return controlPath;
            case "leftMeta": return controlPath;
            case "rightMeta": return controlPath;
            case "contextMenu": return controlPath;
            case "backspace": return controlPath;
            case "pageDown": return controlPath;
            case "pageUp": return controlPath;
            case "home": return controlPath;
            case "end": return controlPath;
            case "insert": return controlPath;
            case "delete": return controlPath;
            case "capsLock": return controlPath;
            case "numLock": return controlPath;
            case "printScreen": return controlPath;
            case "scrollLock": return controlPath;
            case "pause": return controlPath;
            case "numpadEnter": return controlPath;
            case "numpadDivide": return controlPath;
            case "numpadMultiply": return controlPath;
            case "numpadPlus": return controlPath;
            case "numpadMinus": return controlPath;
            case "numpadPeriod": return controlPath;
            case "numpadEquals": return controlPath;
            case "numpad1": return controlPath;
            case "numpad2": return controlPath;
            case "numpad3": return controlPath;
            case "numpad4": return controlPath;
            case "numpad5": return controlPath;
            case "numpad6": return controlPath;
            case "numpad7": return controlPath;
            case "numpad8": return controlPath;
            case "numpad9": return controlPath;
            case "numpad0": return controlPath;
            case "f1": return controlPath;
            case "f2": return controlPath;
            case "f3": return controlPath;
            case "f4": return controlPath;
            case "f5": return controlPath;
            case "f6": return controlPath;
            case "f7": return controlPath;
            case "f8": return controlPath;
            case "f9": return controlPath;
            case "f10": return controlPath;
            case "f11": return controlPath;
            case "f12": return controlPath;
            //mouse bindings
            case "leftButton": return controlPath;
            case "rightButton": return controlPath;
            case "middleButton": return controlPath;
            case "forwardButton": return controlPath;
            //gamepad bindings
            case "backButton": return controlPath;
            case "buttonSouth": return controlPath;
            case "buttonNorth": return controlPath;
            case "buttonEast": return controlPath;
            case "buttonWest": return controlPath;
            case "start": return controlPath;
            case "select": return controlPath;
            case "leftTrigger": return controlPath;
            case "rightTrigger": return controlPath;
            case "leftShoulder": return controlPath;
            case "rightShoulder": return controlPath;
            case "dpad": return controlPath;
            case "dpad/up": return controlPath;
            case "dpad/down": return controlPath;
            case "dpad/left": return controlPath;
            case "dpad/right": return controlPath;
            case "leftStick": return controlPath;
            case "rightStick": return controlPath;
            case "leftStickPress": return controlPath;
            case "rightStickPress": return controlPath;
            //if key is localization dependent (letter keys and some other specific ones) display the binding display string
            default:
                return bindingDisplayString;
        }
    }
}
