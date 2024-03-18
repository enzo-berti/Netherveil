using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Samples.RebindUI;

public class Keybinding : MonoBehaviour
{
    [SerializeField] GameObject KBPanel;
    [SerializeField] GameObject GamepadPanel;

    void Start()
    {
        DeviceManager.OnChangedToKB += SwitchBindings;
        DeviceManager.OnChangedToGamepad += SwitchBindings;
    }

    private void OnDestroy()
    {
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
