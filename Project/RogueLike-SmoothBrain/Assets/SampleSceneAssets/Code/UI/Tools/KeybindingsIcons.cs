using System;
using TMPro;
using UnityEngine.UI;

////TODO: have updateBindingUIEvent receive a control path string, too (in addition to the device layout name)

namespace UnityEngine.InputSystem.Samples.RebindUI
{
    /// <summary>
    /// This is an example for how to override the default display behavior of bindings. The component
    /// hooks into <see cref="RebindActionUI.updateBindingUIEvent"/> which is triggered when UI display
    /// of a binding should be refreshed. It then checks whether we have an icon for the current binding
    /// and if so, replaces the default text display with an icon.
    /// </summary>
    public class KeybindingsIcons : MonoBehaviour
    {
        public GamepadIconsSprites xbox;
        public GamepadIconsSprites ps4;
        public KeyboardIconsSprites kb;
        public MouseIconsSprites mouse;

        protected void Start()
        {
            // Hook into all updateBindingUIEvents on all RebindActionUI components in our hierarchy.
            var rebindUIComponents = FindObjectsOfType<RebindActionUI>(true);
            foreach (var component in rebindUIComponents)
            {
                component.updateBindingUIEvent.AddListener(OnUpdateBindingDisplay);
                component.UpdateBindingDisplay();
            }
        }

        protected void OnUpdateBindingDisplay(RebindActionUI component, string bindingDisplayString, string deviceLayoutName, string controlPath)
        {
            if (string.IsNullOrEmpty(deviceLayoutName) || string.IsNullOrEmpty(controlPath))
                return;

            var icon = default(Sprite);
            
            if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "DualShockGamepad"))
                icon = ps4.GetSprite(controlPath);
            else if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "Gamepad"))
                icon = xbox.GetSprite(controlPath);
            else if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "Keyboard"))
                icon = kb.GetSprite(controlPath);
            else if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "Mouse"))
                icon = mouse.GetSprite(controlPath);


            var textComponent = component.bindingText;

            // Grab Image component.
            var imageGO = textComponent.transform.parent.Find("ActionBindingIcon");

            if (imageGO != null)
            {
                var imageComponent = imageGO.GetComponent<Image>();
                var textMesh = imageComponent.GetComponentInChildren<TMP_Text>();

                if (icon != null)
                {
                    textComponent.gameObject.SetActive(false);
                    imageComponent.sprite = icon;
                    imageComponent.gameObject.SetActive(true);

                    if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "Keyboard"))
                        textMesh.text = GetAppropriateKeyString(controlPath, bindingDisplayString);
                    else if (textMesh != null)
                        textMesh.text = string.Empty;
                }
                else
                {
                    imageComponent.gameObject.SetActive(false);
                    textComponent.gameObject.SetActive(true);
                }
            }
        }

        private string GetAppropriateKeyString(string controlPath, string bindingDisplayString)
        {
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

        [Serializable]
        public struct GamepadIconsSprites

        {
            public Sprite buttonSouth;
            public Sprite buttonNorth;
            public Sprite buttonEast;
            public Sprite buttonWest;
            public Sprite startButton;
            public Sprite selectButton;
            public Sprite leftTrigger;
            public Sprite rightTrigger;
            public Sprite leftShoulder;
            public Sprite rightShoulder;
            public Sprite dpad;
            public Sprite dpadUp;
            public Sprite dpadDown;
            public Sprite dpadLeft;
            public Sprite dpadRight;
            public Sprite leftStick;
            public Sprite rightStick;
            public Sprite leftStickPress;
            public Sprite rightStickPress;

            public Sprite GetSprite(string controlPath)
            {
                // From the input system, we get the path of the control on device. So we can just
                // map from that to the sprites we have for gamepads.
                switch (controlPath)
                {
                    case "buttonSouth": return buttonSouth;
                    case "buttonNorth": return buttonNorth;
                    case "buttonEast": return buttonEast;
                    case "buttonWest": return buttonWest;
                    case "start": return startButton;
                    case "select": return selectButton;
                    case "leftTrigger": return leftTrigger;
                    case "rightTrigger": return rightTrigger;
                    case "leftShoulder": return leftShoulder;
                    case "rightShoulder": return rightShoulder;
                    case "dpad": return dpad;
                    case "dpad/up": return dpadUp;
                    case "dpad/down": return dpadDown;
                    case "dpad/left": return dpadLeft;
                    case "dpad/right": return dpadRight;
                    case "leftStick": return leftStick;
                    case "rightStick": return rightStick;
                    case "leftStickPress": return leftStickPress;
                    case "rightStickPress": return rightStickPress;
                }
                return null;
            }
        }

        [Serializable]
        public struct KeyboardIconsSprites

        {
            public Sprite buttonLong;
            public Sprite buttonMedium;
            public Sprite buttonSimple;
            public Sprite buttonCorner;

            public Sprite GetSprite(string controlPath)
            {
                // From the input system, we get the path of the control on device. So we can just
                // map from that to the sprites we have for gamepads.

                switch (controlPath)
                {
                    case "enter": return buttonCorner;
                    case "escape": return buttonMedium;
                    case "backspace": return buttonMedium;
                    case "rightShift": return buttonMedium;
                    case "leftShift": return buttonMedium;
                    case "rightCtrl": return buttonMedium;
                    case "leftCtrl": return buttonMedium;
                    case "capsLock": return buttonMedium;
                    case "tab": return buttonMedium;
                    case "space": return buttonLong;
                    default: return buttonSimple;
                }
            }
        }

        [Serializable]
        public struct MouseIconsSprites

        {
            public Sprite mouse;
            public Sprite mouseLeft;
            public Sprite mouseRight;
            public Sprite mouseScroll;

            public Sprite GetSprite(string controlPath)
            {
                // From the input system, we get the path of the control on device. So we can just
                // map from that to the sprites we have for gamepads.

                switch (controlPath)
                {
                    case "leftButton": return mouseLeft;
                    case "rightButton": return mouseRight;
                    case "middleButton": return mouseScroll;
                    default: return mouse;
                }
            }
        }
    }
}
