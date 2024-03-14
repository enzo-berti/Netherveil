using UnityEngine;
using UnityEngine.InputSystem.Samples.RebindUI;

public class Keybinding : MonoBehaviour
{
    [SerializeField] RebindActionUI dashRebind;

    void Start()
    {
        DeviceManager.OnChangedToKB += SwitchBindings;
        DeviceManager.OnChangedToGamepad += SwitchBindings;
    }

    private void OnDestroy()
    {
        DeviceManager.OnChangedToKB -= SwitchBindings;
        DeviceManager.OnChangedToGamepad -= SwitchBindings;
    }

    private void SwitchBindings()
    {
        int value = DeviceManager.Instance.IsPlayingKB() ? 0 : 1;

        //dashRebind.bindingId = dashRebind.actionReference.action.bindings[value].id.ToString();
    }
}
