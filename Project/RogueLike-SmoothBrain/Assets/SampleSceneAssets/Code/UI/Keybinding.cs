using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Keybinding : MonoBehaviour
{
    [SerializeField] List<GameObject> KBBindings;
    [SerializeField] List<GameObject> GamepadBindings;
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
            bool isPlayingKB = DeviceManager.Instance.IsPlayingKB();

            for (int i = 0; i < KBBindings.Count; ++i)
            {
                KBBindings[i].SetActive(isPlayingKB);
                GamepadBindings[i].SetActive(!isPlayingKB);
            }
        }
    }
}
