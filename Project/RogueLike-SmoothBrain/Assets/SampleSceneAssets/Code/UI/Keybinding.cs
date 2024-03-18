using UnityEngine;
using UnityEngine.InputSystem;

public class Keybinding : MonoBehaviour
{
    [SerializeField] GameObject KBPanel;
    [SerializeField] GameObject GamepadPanel;
    [SerializeField] UnityEngine.InputSystem.PlayerInput playerInput;

    void Start()
    {
        DeviceManager.OnChangedToKB += SwitchBindings;
        DeviceManager.OnChangedToGamepad += SwitchBindings;
    }

    private void OnDestroy()
    {
    }

    public void ResetBindings()
    {
        playerInput.currentActionMap.RemoveAllBindingOverrides();
    }

    private void SwitchBindings()
    {
        if (this != null)
        {
            if (DeviceManager.Instance.IsPlayingKB())
            {
                GamepadPanel.SetActive(false);
                KBPanel.SetActive(true);
            }
            else
            {
                GamepadPanel.SetActive(true);
                KBPanel.SetActive(false);
            }
        }
    }
}
