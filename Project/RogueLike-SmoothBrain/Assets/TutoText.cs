using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutoText : MonoBehaviour
{
    [SerializeField] List<InputActionReference> keyboardActions;
    [SerializeField] List<InputActionReference> gamepadActions;
    [SerializeField] InputBinding.DisplayStringOptions displayStringOptions;

    string initText = string.Empty;
    TMP_Text text;

    void Start()
    {
        text = GetComponent<TMP_Text>();
        initText = text.text;
        UpdateBindingDisplayString();
        DeviceManager.OnChangedToKB += UpdateBindingDisplayString;
        DeviceManager.OnChangedToGamepad += UpdateBindingDisplayString;
    }

    private void OnDestroy()
    {
        DeviceManager.OnChangedToKB -= UpdateBindingDisplayString;
        DeviceManager.OnChangedToGamepad -= UpdateBindingDisplayString;
    }

    private void UpdateBindingDisplayString()
    {
        List<InputActionReference> actionRefs = GetCurrentAction();

        int nbDifferentTexts = 0;
        for(int i = 0; i < actionRefs.Count; i++)
        {
            if (actionRefs[i].action.GetBindingDisplayString(displayStringOptions) != text.text)
                nbDifferentTexts++;
        }

        if(nbDifferentTexts > 0)
        {
            string textString = initText;
            textString = textString.Replace("^", GetDisplayString(actionRefs[0]));
            if(textString.Contains("$"))
            {
                textString = textString.Replace("$", GetDisplayString(actionRefs[1]));
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

    private string GetDisplayString(InputActionReference actionRef)
    {
        // Get display string from action.
        var action = actionRef != null ? actionRef.action : null;
        string displayString = string.Empty;

        if (action != null)
        {
            if (!DeviceManager.Instance.IsPlayingKB() && actionRef.action.name == "Movement")
            {
                displayString = "Left Stick";
            }
            else
            {
                string displayStringTMP = action.GetBindingDisplayString(0, out _, out string controlPath, displayStringOptions);
                displayString = displayStringTMP.Length == 1 || displayStringTMP.Contains("MB") || actionRef.action.name == "Movement" ? displayStringTMP : controlPath;
            }
        }

        return displayString;
    }
}
