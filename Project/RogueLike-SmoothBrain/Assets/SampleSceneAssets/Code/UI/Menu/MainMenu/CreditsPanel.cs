using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CreditsPanel : MonoBehaviour
{
    [SerializeField] private Selectable selectable;
    public void EnableCreditPannel(bool enable)
    {
        if(enable)
        {
            DeviceManager.OnChangedToGamepad += SetSelectable;
            DeviceManager.OnChangedToKB += UnsetSelectable;
        }
        else
        {
            DeviceManager.OnChangedToGamepad -= SetSelectable;
            DeviceManager.OnChangedToKB -= UnsetSelectable;
        }
    }

    private void SetSelectable()
    {
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(selectable.gameObject);
    }
    private void UnsetSelectable()
    {
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
    }
}
